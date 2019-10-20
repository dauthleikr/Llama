namespace Llama.Compiler.ExpressionCompilers
{
    using Extensions;
    using Parser.Nodes;
    using spit;

    internal class TypeCastCompiler : ICompileExpressions<TypeCastExpression>
    {
        public ExpressionResult Compile(
            TypeCastExpression expression,
            PreferredRegister target,
            CodeGen codeGen,
            StorageManager storageManager,
            IScopeContext scope,
            IAddressFixer addressFixer,
            ICompilationContext context
        )
        {
            var source = context.CompileExpression(expression.CastExpression, codeGen, storageManager, target, scope);

            var sourceType = source.ValueType;
            var targetType = expression.Type;

            if (sourceType == targetType)
                return source;

            if (CanCastUnsafe(targetType, sourceType))
            {
                source.ChangeTypeUnsafe(targetType);
                return source;
            }

            var targetRegister = target.MakeFor(targetType);
            if (targetType.ChildRelation == Type.WrappingType.PointerOf && // Allow array to pointer casts if the underlying type is identical
                sourceType.ChildRelation == Type.WrappingType.ArrayOf &&
                sourceType.Child == targetType.Child)
            {
                source.GenerateMoveTo(targetRegister, codeGen, addressFixer);
                codeGen.Add(targetRegister.AsR64(), (sbyte)8);
                return new ExpressionResult(targetType, targetRegister);
            }

            // we're done with pointer and array casting. Only value casting down below
            if (targetType.ChildRelation != Type.WrappingType.None || sourceType.ChildRelation != Type.WrappingType.None)
                throw new BadCastException($"Cannot cast {sourceType} to {targetType}");

            var sourceTypeSize = sourceType.SizeOf();
            var targetTypeSize = targetType.SizeOf();
            var sourceIsInt = sourceType.IsIntegerRegisterType();
            var targetIsInt = targetType.IsIntegerRegisterType();

            if (sourceIsInt && targetIsInt)
            {
                if (targetTypeSize > sourceTypeSize)
                {
                    // int register widening - works implicitly - isCast flag given to do signed <-> unsigned conversion by force
                    source.GenerateMoveTo(targetRegister, targetType, codeGen, addressFixer, true);
                    return new ExpressionResult(targetType, targetRegister);
                }

                // int register narrowing
                source.GenerateMoveTo(targetRegister, codeGen, addressFixer);
                // // clean rest of register
                //if (targetTypeSize == 4)
                //    codeGen.Mov(targetRegister.AsR32(), targetRegister.AsR32());
                //else
                //    codeGen.And(targetRegister.AsR32(), targetTypeSize == 2 ? ushort.MaxValue : byte.MaxValue);
                return new ExpressionResult(targetType, targetRegister);
            }

            if (!sourceIsInt && !targetIsInt) // float <-> double conversions
            {
                source.GenerateMoveTo(targetRegister, codeGen, addressFixer);
                if (targetTypeSize < sourceTypeSize)
                    codeGen.CvtSd2Ss(targetRegister.AsFloat(), targetRegister.AsFloat());
                else
                    codeGen.CvtSs2Sd(targetRegister.AsFloat(), targetRegister.AsFloat());
                return new ExpressionResult(targetType, targetRegister);
            }

            if (!sourceIsInt) /* && targetIsInt) */
                return CastFloatToInt(sourceType, targetType, targetRegister, source, codeGen, addressFixer);

            /* if (!targetIsInt && sourceIsInt) */
            return CastIntToFloat(sourceType, targetType, targetRegister, source, codeGen, addressFixer);
        }

        private static ExpressionResult CastIntToFloat(
            Type sourceType,
            Type targetType,
            Register targetRegister,
            ExpressionResult source,
            CodeGen codeGen,
            IAddressFixer addressFixer
        )
        {
            var sourceTypeSize = sourceType.SizeOf();
            var tempSource = Register.IntRegisterFromSize(sourceTypeSize, 0);
            source.GenerateMoveTo(tempSource, Constants.LongType, codeGen, addressFixer);

            if (targetType == Constants.FloatType)
            {
                switch (sourceTypeSize)
                {
                    case 8:
                        codeGen.CvtSi2Ss(targetRegister.AsFloat(), tempSource.AsR64());
                        break;
                    case 4:
                        codeGen.CvtSi2Ss(targetRegister.AsFloat(), tempSource.AsR32());
                        break;
                    default:
                        throw new BadCastException($"Cannot cast {sourceType} to {targetType}");
                }
            }

            if (targetType == Constants.DoubleType)
            {
                switch (sourceTypeSize)
                {
                    case 8:
                        codeGen.CvtSi2Sd(targetRegister.AsFloat(), tempSource.AsR64());
                        break;
                    case 4:
                        codeGen.CvtSi2Sd(targetRegister.AsFloat(), tempSource.AsR32());
                        break;
                    default:
                        throw new BadCastException($"Cannot cast {sourceType} to {targetType}");
                }
            }
            else
                throw new BadCastException($"Cannot cast {sourceType} to {targetType}");

            return new ExpressionResult(targetType, targetRegister);
        }

        private static ExpressionResult CastFloatToInt(
            Type sourceType,
            Type targetType,
            Register targetRegister,
            ExpressionResult source,
            CodeGen codeGen,
            IAddressFixer addressFixer
        )
        {
            source.GenerateMoveTo(XmmRegister.XMM0, sourceType, codeGen, addressFixer);
            if (sourceType == Constants.FloatType)
            {
                switch (targetType.SizeOf())
                {
                    case 8:
                        codeGen.CvtSs2Si(targetRegister.AsR64(), XmmRegister.XMM0);
                        break;
                    case 4:
                        codeGen.CvtSs2Si(targetRegister.AsR32(), XmmRegister.XMM0);
                        break;
                    default:
                        throw new BadCastException($"Cannot cast {sourceType} to {targetType}");
                }
            }

            if (sourceType == Constants.DoubleType)
            {
                switch (targetType.SizeOf())
                {
                    case 8:
                        codeGen.CvtSd2Si(targetRegister.AsR64(), XmmRegister.XMM0);
                        break;
                    case 4:
                        codeGen.CvtSd2Si(targetRegister.AsR32(), XmmRegister.XMM0);
                        break;
                    default:
                        throw new BadCastException($"Cannot cast {sourceType} to {targetType}");
                }
            }
            else
                throw new BadCastException($"Cannot cast {sourceType} to {targetType}");

            return new ExpressionResult(targetType, targetRegister);
        }

        private static bool CanCastUnsafe(Type targetType, Type sourceType)
        {
            if (sourceType == Constants.CstrType &&
                targetType.ChildRelation == Type.WrappingType.PointerOf &&
                targetType.Child.ChildRelation == Type.WrappingType.None &&
                targetType.Child.SizeOf() == 1)
                return true; // allow cstr -> any single byte ptr
            if (targetType == Constants.CstrType &&
                sourceType.ChildRelation == Type.WrappingType.PointerOf &&
                sourceType.Child.ChildRelation == Type.WrappingType.None &&
                sourceType.Child.SizeOf() == 1)
                return true; // allow any single byte ptr -> cstr

            if (sourceType.ChildRelation == Type.WrappingType.PointerOf && targetType.ChildRelation == Type.WrappingType.PointerOf)
                return true; // Can cast any pointer to any pointer
            if (sourceType.ChildRelation == Type.WrappingType.ArrayOf &&
                targetType.ChildRelation == Type.WrappingType.ArrayOf &&
                sourceType.Child.SizeOf() == targetType.Child.SizeOf() &&
                sourceType.Child.Child == targetType.Child.Child)
                return true; // Can cast array to other array of same size
            return false;
        }
    }
}