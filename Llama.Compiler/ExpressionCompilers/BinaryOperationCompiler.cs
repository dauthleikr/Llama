namespace Llama.Compiler.ExpressionCompilers
{
    using System;
    using Extensions;
    using Parser.Lexer;
    using Parser.Nodes;
    using spit;
    using Type = Parser.Nodes.Type;

    internal class BinaryOperationCompiler : ICompileExpressions<BinaryOperatorExpression>
    {
        public ExpressionResult Compile(
            BinaryOperatorExpression expression,
            PreferredRegister target,
            ICompilationContext context
        )
        {
            switch (expression.Operator.Operator.Kind)
            {
                case TokenKind.Plus:
                    return CompileAny(
                        expression.Left,
                        expression.Right,
                        ExpressionResultExtensions.AddTo,
                        ExpressionResultExtensions.AddsdTo,
                        ExpressionResultExtensions.AddssTo,
                        target,
                        context
                    );
                case TokenKind.Minus:
                    return CompileAny(
                        expression.Left,
                        expression.Right,
                        ExpressionResultExtensions.SubTo,
                        ExpressionResultExtensions.SubsdTo,
                        ExpressionResultExtensions.SubssTo,
                        target,
                        context
                    );
                case TokenKind.Pointer:
                    return CompileMultiply(expression.Left, expression.Right, target, context);
                case TokenKind.Divide:
                    return CompileDivide(expression.Left, expression.Right, target, context);
                case TokenKind.Modolu:
                    return CompileModolu(expression.Left, expression.Right, target, context);
                case TokenKind.Assignment:
                    return CompileAssign(expression.Left, expression.Right, target, context);
                case TokenKind.Equals:
                    return CompileComparison(
                        expression.Left,
                        expression.Right,
                        target,
                        context,
                        context.Generator.Je,
                        context.Generator.Je
                    );
                case TokenKind.NotEquals:
                    return CompileComparison(
                        expression.Left,
                        expression.Right,
                        target,
                        context,
                        context.Generator.Jne,
                        context.Generator.Jne
                    );
                case TokenKind.OpenAngularBracket:
                    return CompileComparison(
                        expression.Left,
                        expression.Right,
                        target,
                        context,
                        context.Generator.Jl,
                        context.Generator.Jb
                    );
                case TokenKind.CloseAngularBracket:
                    return CompileComparison(
                        expression.Left,
                        expression.Right,
                        target,
                        context,
                        context.Generator.Jg,
                        context.Generator.Ja
                    );
                case TokenKind.GreaterEquals:
                    return CompileComparison(
                        expression.Left,
                        expression.Right,
                        target,
                        context,
                        context.Generator.Jge,
                        context.Generator.Jae
                    );
                case TokenKind.SmallerEquals:
                    return CompileComparison(
                        expression.Left,
                        expression.Right,
                        target,
                        context,
                        context.Generator.Jbe,
                        context.Generator.Jle
                    );
                default:
                    throw new NotImplementedException($"Compilation for Operator {expression.Operator.Operator.Kind} is not implemented");
            }
        }

        private static ExpressionResult CompileAny(
            IExpression left,
            IExpression right,
            Action<ExpressionResult, Register, CodeGen, ILinkingInfo> actionInt,
            Action<ExpressionResult, Register, CodeGen, ILinkingInfo> actionFloat,
            Action<ExpressionResult, Register, CodeGen, ILinkingInfo> actionDouble,
            PreferredRegister target,
            ICompilationContext context
        )
        {
            var (leftReg, rightExpr, type) = PrepareBinaryExpression(
                left,
                right,
                target,
                context,
                true
            );

            if (type.IsIntegerRegisterType())
                actionInt(rightExpr, leftReg, context.Generator, context.Linking);
            else if (type == Constants.DoubleType)
                actionDouble(rightExpr, leftReg, context.Generator, context.Linking);
            else if (type == Constants.FloatType)
                actionFloat(rightExpr, leftReg, context.Generator, context.Linking);
            else
            {
                throw new NotImplementedException(
                    $"{nameof(BinaryOperationCompiler)}: {nameof(CompileAny)}: I do not know how to compile this type: {type}"
                );
            }

            return new ExpressionResult(type, leftReg);
        }

        private static ExpressionResult CompileMultiply(
            IExpression left,
            IExpression right,
            PreferredRegister target,
            ICompilationContext context
        )
        {
            var tempRegister = new PreferredRegister(Register64.RAX, target.FloatRegister);
            var (leftReg, rightExpr, type) = PrepareBinaryExpression(
                left,
                right,
                tempRegister,
                context,
                true
            );

            if (type.IsIntegerRegisterType())
            {
                if (type.SizeOf() == 1)
                    throw new NotImplementedException("Multiplications with 8-bit types are not implemented");

                rightExpr.ImulTo(leftReg, context.Generator, context.Linking);
            }
            else if (type == Constants.DoubleType)
                rightExpr.MulsdTo(leftReg, context.Generator, context.Linking);
            else if (type == Constants.FloatType)
                rightExpr.MulssTo(leftReg, context.Generator, context.Linking);
            else
            {
                throw new NotImplementedException(
                    $"{nameof(BinaryOperationCompiler)}: {nameof(CompileMultiply)}: I do not know how to compile this type: {type}"
                );
            }

            return new ExpressionResult(type, leftReg);
        }

        private static ExpressionResult CompileDivide(
            IExpression left,
            IExpression right,
            PreferredRegister target,
            ICompilationContext context
        )
        {
            var leftRegisterPref = new PreferredRegister(Register64.RAX, target.FloatRegister);
            var (leftReg, rightExpr, type) = PrepareBinaryExpression(
                left,
                right,
                leftRegisterPref,
                context,
                true
            );

            if (type.IsIntegerRegisterType())
            {
                CompileIntegerDivision(context.Generator, context.Linking, type, leftReg, rightExpr);
                return new ExpressionResult(type, new PreferredRegister(Register64.RAX).MakeFor(type));
            }

            if (type == Constants.DoubleType)
                rightExpr.DivsdTo(leftReg, context.Generator, context.Linking);
            else if (type == Constants.FloatType)
                rightExpr.DivssTo(leftReg, context.Generator, context.Linking);
            else
            {
                throw new NotImplementedException(
                    $"{nameof(BinaryOperationCompiler)}: {nameof(CompileDivide)}: I do not know how to compile this type: {type}"
                );
            }

            return new ExpressionResult(type, leftReg);
        }

        private static ExpressionResult CompileModolu(
            IExpression left,
            IExpression right,
            PreferredRegister target,
            ICompilationContext context
        )
        {
            var leftRegisterPref = new PreferredRegister(Register64.RAX, target.FloatRegister);
            var (leftReg, rightExpr, type) = PrepareBinaryExpression(
                left,
                right,
                leftRegisterPref,
                context,
                true
            );

            if (type.IsIntegerRegisterType())
            {
                var typeIsByte = CompileIntegerDivision(context.Generator, context.Linking, type, leftReg, rightExpr);
                if (typeIsByte) // for byte op. the remainder is stored in 'ah' instead
                    context.Generator.Write(0x8A, 0xD4); // mov dl, ah

                return new ExpressionResult(type, new PreferredRegister(Register64.RDX).MakeFor(type));
            }

            throw new NotImplementedException(
                $"{nameof(BinaryOperationCompiler)}: {nameof(CompileModolu)}: I do not know how to compile this type: {type}"
            );
        }

        private static bool CompileIntegerDivision(CodeGen codeGen, ILinkingInfo linkingInfo, Type type, Register leftReg, ExpressionResult rightExpr)
        {
            var rightReg = type.OtherVolatileIntRegister(leftReg, Register64.RAX, Register64.RDX);
            rightExpr.GenerateMoveTo(rightReg, type, codeGen, linkingInfo);

            if (!leftReg.IsSameRegister(Register64.RAX))
                codeGen.Mov(Register64.RAX, leftReg.AsR64());

            var typeIsByte = type.SizeOf() == 1;
            var typeIsSigned = type.IsSignedInteger();
            if (typeIsByte)
            {
                if (typeIsSigned)
                    codeGen.Movsx(Register64.RAX, Register8.AL);
                else
                    codeGen.Movzx(Register64.RAX, Register8.AL);
            }
            else
            {
                codeGen.Xor(Register64.RDX, Register64.RDX);
                if (typeIsSigned)
                {
                    codeGen.Test(Register64.RAX, Register64.RAX);
                    codeGen.Jns(CodeGenExtensions.InstructionLength(gen => gen.Not(Register64.RDX)));
                    codeGen.Not(Register64.RDX);
                }
            }

            if (type.IsSignedInteger())
                codeGen.Idiv(rightReg);
            else
                codeGen.Div(rightReg);
            return typeIsByte;
        }

        private static ExpressionResult CompileAssign(IExpression left, IExpression right, PreferredRegister target, ICompilationContext context)
        {
            var (expression, assign, type) = PrepareBinaryExpression(right, left, target, context);
            assign.GenerateAssign(expression, context.Generator, context.Linking);
            return assign;
        }

        private static ExpressionResult CompileComparison(
            IExpression left,
            IExpression right,
            PreferredRegister target,
            ICompilationContext context,
            Action<sbyte> comparisonJmpSigned,
            Action<sbyte> comparisonJmpUnsignedd,
            bool inverted = false
        )
        {
            var (leftReg, rightExpr, type) = PrepareBinaryExpression(
                left,
                right,
                target,
                context,
                true
            );
            if (type.IsIntegerRegisterType())
                rightExpr.CmpTo(leftReg, context.Generator, context.Linking);
            else if (type == Constants.DoubleType)
                rightExpr.ComisdTo(leftReg, context.Generator, context.Linking);
            else if (type == Constants.FloatType)
                rightExpr.ComissTo(leftReg, context.Generator, context.Linking);
            else
            {
                throw new NotImplementedException(
                    $"{nameof(BinaryOperationCompiler)}: {nameof(CompileComparison)}: I do not know how to compile this type: {type}"
                );
            }

            var targetRegister = target.MakeFor(Constants.BoolType);

            var mov1CodeGen = new CodeGen();
            mov1CodeGen.Mov(targetRegister.AsR32(), inverted ? 0 : 1);
            var mov0CodeGen = new CodeGen();
            mov0CodeGen.Mov(targetRegister.AsR32(), inverted ? 1 : 0);
            mov0CodeGen.Jmp(mov1CodeGen.GetBufferSpan().Length);

            if (type.IsSignedInteger())
                comparisonJmpSigned((sbyte)mov0CodeGen.GetBufferSpan().Length);
            else
                comparisonJmpUnsignedd((sbyte)mov0CodeGen.GetBufferSpan().Length);

            context.Generator.Write(mov0CodeGen.GetBufferSpan());
            context.Generator.Write(mov1CodeGen.GetBufferSpan());
            return new ExpressionResult(Constants.BoolType, targetRegister);
        }

        /// <summary>
        ///     Evaluates two expressions, and brings them to a common type. Tries to keep the second expression as reference, for
        ///     mor optimal code generation.
        /// </summary>
        /// <param name="first"></param>
        /// <param name="second"></param>
        /// <param name="preferredFirst"></param>
        /// <param name="codeGen"></param>
        /// <param name="storageManager"></param>
        /// <param name="scope"></param>
        /// <param name="linkingInfo"></param>
        /// <param name="context"></param>
        /// <param name="shouldDereferenceForImplicitCast">The reference may be deferenced to cast it to the common type</param>
        /// <returns></returns>
        private static (Register register, ExpressionResult reference, Type type) PrepareBinaryExpression(
            IExpression first,
            IExpression second,
            PreferredRegister preferredFirst,
            ICompilationContext context,
            bool shouldDereferenceForImplicitCast = false
        )
        {
            var firstResult = context.CompileExpression(first, preferredFirst);
            var isfirstIntegerType = firstResult.ValueType.IsIntegerRegisterType();
            var firstTemp = context.Storage.Allocate(isfirstIntegerType);
            firstTemp.Store(firstResult, context.Generator, context.Linking);

            var secondResult = context.CompileExpression(second, preferredFirst);
            var type = firstResult.ValueType;
            if (firstResult.ValueType != secondResult.ValueType)
            {
                if (secondResult.ValueType.CanAssignImplicitly(firstResult.ValueType))
                    type = secondResult.ValueType;
                else if (shouldDereferenceForImplicitCast && firstResult.ValueType.CanAssignImplicitly(secondResult.ValueType))
                {
                    type = firstResult.ValueType;

                    var secondDerefVolatile = secondResult.GetOccupiedOrVolatile(type);
                    secondResult.GenerateMoveTo(secondDerefVolatile, type, context.Generator, context.Linking);
                    secondResult = new ExpressionResult(type, secondDerefVolatile);
                }
                else
                    throw new TypeMismatchException("Any common type", $"{firstResult.ValueType} and {secondResult.ValueType}");
            }

            var preferredRegister = preferredFirst.MakeFor(type);
            var firstRegister = secondResult.IsOccopied(preferredRegister) ? secondResult.GetUnoccupiedVolatile(type) : preferredRegister;
            firstTemp.AsExpressionResult(firstResult.ValueType).GenerateMoveTo(firstRegister, type, context.Generator, context.Linking);
            context.Storage.Release(firstTemp);
            return (firstRegister, secondResult, type);
        }
    }
}