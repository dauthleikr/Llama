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
            CodeGen codeGen,
            StorageManager storageManager,
            IScopeContext scope,
            IAddressFixer addressFixer,
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
                        codeGen,
                        storageManager,
                        scope,
                        addressFixer,
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
                        codeGen,
                        storageManager,
                        scope,
                        addressFixer,
                        context
                    );
                case TokenKind.Pointer:
                    return CompileMultiply(expression.Left, expression.Right, target, codeGen, storageManager, scope, addressFixer, context);
                case TokenKind.Divide:
                    return CompileDivide(expression.Left, expression.Right, target, codeGen, storageManager, scope, addressFixer, context);
                case TokenKind.Modolu:
                    return CompileModolu(expression.Left, expression.Right, target, codeGen, storageManager, scope, addressFixer, context);
                case TokenKind.Assignment:
                    return CompileAssign(expression.Left, expression.Right, target, codeGen, storageManager, scope, addressFixer, context);
                case TokenKind.Equals:
                    return CompileComparison(
                        expression.Left,
                        expression.Right,
                        target,
                        codeGen,
                        storageManager,
                        scope,
                        addressFixer,
                        context,
                        codeGen.Je,
                        codeGen.Je
                    );
                case TokenKind.NotEquals:
                    return CompileComparison(
                        expression.Left,
                        expression.Right,
                        target,
                        codeGen,
                        storageManager,
                        scope,
                        addressFixer,
                        context,
                        codeGen.Jne,
                        codeGen.Jne
                    );
                case TokenKind.OpenAngularBracket:
                    return CompileComparison(
                        expression.Left,
                        expression.Right,
                        target,
                        codeGen,
                        storageManager,
                        scope,
                        addressFixer,
                        context,
                        codeGen.Jl,
                        codeGen.Jb
                    );
                case TokenKind.CloseAngularBracket:
                    return CompileComparison(
                        expression.Left,
                        expression.Right,
                        target,
                        codeGen,
                        storageManager,
                        scope,
                        addressFixer,
                        context,
                        codeGen.Jg,
                        codeGen.Ja
                    );
                case TokenKind.GreaterEquals:
                    return CompileComparison(
                        expression.Left,
                        expression.Right,
                        target,
                        codeGen,
                        storageManager,
                        scope,
                        addressFixer,
                        context,
                        codeGen.Jge,
                        codeGen.Jae
                    );
                case TokenKind.SmallerEquals:
                    return CompileComparison(
                        expression.Left,
                        expression.Right,
                        target,
                        codeGen,
                        storageManager,
                        scope,
                        addressFixer,
                        context,
                        codeGen.Jbe,
                        codeGen.Jle
                    );
                default:
                    throw new NotImplementedException($"Compilation for Operator {expression.Operator.Operator.Kind} is not implemented");
            }
        }

        private static ExpressionResult CompileAny(
            IExpression left,
            IExpression right,
            Action<ExpressionResult, Register, CodeGen, IAddressFixer> actionInt,
            Action<ExpressionResult, Register, CodeGen, IAddressFixer> actionFloat,
            Action<ExpressionResult, Register, CodeGen, IAddressFixer> actionDouble,
            PreferredRegister target,
            CodeGen codeGen,
            StorageManager storageManager,
            IScopeContext scope,
            IAddressFixer addressFixer,
            ICompilationContext context
        )
        {
            var (leftReg, rightExpr, type) = PrepareBinaryExpression(left, right, target, codeGen, storageManager, scope, addressFixer, context);

            if (type.IsIntegerRegisterType())
                actionInt(rightExpr, leftReg, codeGen, addressFixer);
            else if (type == Constants.DoubleType)
                actionDouble(rightExpr, leftReg, codeGen, addressFixer);
            else if (type == Constants.FloatType)
                actionFloat(rightExpr, leftReg, codeGen, addressFixer);
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
            CodeGen codeGen,
            StorageManager storageManager,
            IScopeContext scope,
            IAddressFixer addressFixer,
            ICompilationContext context
        )
        {
            var tempRegister = new PreferredRegister(Register64.RAX, target.FloatRegister);
            var (leftReg, rightExpr, type) = PrepareBinaryExpression(
                left,
                right,
                tempRegister,
                codeGen,
                storageManager,
                scope,
                addressFixer,
                context
            );

            if (type.IsIntegerRegisterType())
            {
                if (type.SizeOf() == 1)
                    throw new NotImplementedException("Multiplications with 8-bit types are not implemented");

                rightExpr.ImulTo(leftReg, codeGen, addressFixer);
            }
            else if (type == Constants.DoubleType)
                rightExpr.DivsdTo(leftReg, codeGen, addressFixer);
            else if (type == Constants.FloatType)
                rightExpr.DivssTo(leftReg, codeGen, addressFixer);
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
            CodeGen codeGen,
            StorageManager storageManager,
            IScopeContext scope,
            IAddressFixer addressFixer,
            ICompilationContext context
        )
        {
            var leftRegisterPref = new PreferredRegister(Register64.RAX, target.FloatRegister);
            var rightRegisterPref = new PreferredRegister(Register64.RCX, target.FloatRegister);
            var (leftReg, rightExpr, type) = PrepareBinaryExpression(
                left,
                right,
                leftRegisterPref,
                codeGen,
                storageManager,
                scope,
                addressFixer,
                context
            );

            if (type.IsIntegerRegisterType())
            {
                var rightReg = rightRegisterPref.MakeFor(type);
                if (!leftReg.IsSameRegister(Register64.RAX))
                    codeGen.Mov(Register64.RAX, leftReg.AsR64());

                rightExpr.GenerateMoveTo(rightReg, type, codeGen, addressFixer);

                var typeIsByte = type.SizeOf() == 1;
                if (typeIsByte) // clear remainder space
                    codeGen.Movzx(Register64.RAX, Register8.AL);
                else
                    codeGen.Xor(Register64.RDX, Register64.RDX);

                if (type.IsSignedInteger())
                    codeGen.Idiv(rightReg);
                else
                    codeGen.Div(rightReg);

                return new ExpressionResult(type, leftReg);
            }

            if (type == Constants.DoubleType)
                rightExpr.DivsdTo(leftReg, codeGen, addressFixer);
            else if (type == Constants.FloatType)
                rightExpr.DivssTo(leftReg, codeGen, addressFixer);
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
            CodeGen codeGen,
            StorageManager storageManager,
            IScopeContext scope,
            IAddressFixer addressFixer,
            ICompilationContext context
        )
        {
            var leftRegisterPref = new PreferredRegister(Register64.RAX, target.FloatRegister);
            var rightRegisterPref = new PreferredRegister(Register64.RCX, target.FloatRegister);
            var (leftReg, rightExpr, type) = PrepareBinaryExpression(
                left,
                right,
                leftRegisterPref,
                codeGen,
                storageManager,
                scope,
                addressFixer,
                context
            );

            if (type.IsIntegerRegisterType())
            {
                var rightReg = rightRegisterPref.MakeFor(type);
                if (!leftReg.IsSameRegister(Register64.RAX))
                    codeGen.Mov(Register64.RAX, leftReg.AsR64());

                rightExpr.GenerateMoveTo(rightReg, type, codeGen, addressFixer);

                var typeIsByte = type.SizeOf() == 1;
                if (typeIsByte) // clear remainder space
                    codeGen.Movzx(Register64.RAX, Register8.AL);
                else
                    codeGen.Xor(Register64.RDX, Register64.RDX);

                if (type.IsSignedInteger())
                    codeGen.Idiv(rightReg);
                else
                    codeGen.Div(rightReg);

                if (typeIsByte) // for byte op. the remainder is stored in 'ah' instead
                    codeGen.Write(0x8A, 0xD4); // mov dl, ah

                return new ExpressionResult(type, new PreferredRegister(Register64.RDX).MakeFor(type));
            }

            throw new NotImplementedException(
                $"{nameof(BinaryOperationCompiler)}: {nameof(CompileModolu)}: I do not know how to compile this type: {type}"
            );
        }

        private static ExpressionResult CompileAssign(
            IExpression left,
            IExpression right,
            PreferredRegister target,
            CodeGen codeGen,
            StorageManager storageManager,
            IScopeContext scope,
            IAddressFixer addressFixer,
            ICompilationContext context
        )
        {
            var (expression, assign, type) = PrepareBinaryExpression(right, left, target, codeGen, storageManager, scope, addressFixer, context);
            assign.GenerateAssign(expression, codeGen, addressFixer);
            return assign;
        }

        private static ExpressionResult CompileComparison(
            IExpression left,
            IExpression right,
            PreferredRegister target,
            CodeGen codeGen,
            StorageManager storageManager,
            IScopeContext scope,
            IAddressFixer addressFixer,
            ICompilationContext context,
            Action<sbyte> comparisonJmpSigned,
            Action<sbyte> comparisonJmpUnsignedd,
            bool inverted = false
        )
        {
            var (leftReg, rightExpr, type) = PrepareBinaryExpression(left, right, target, codeGen, storageManager, scope, addressFixer, context);
            if (type.IsIntegerRegisterType())
                rightExpr.CmpTo(leftReg, codeGen, addressFixer);
            else if (type == Constants.DoubleType)
                rightExpr.ComisdTo(leftReg, codeGen, addressFixer);
            else if (type == Constants.FloatType)
                rightExpr.ComissTo(leftReg, codeGen, addressFixer);
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

            codeGen.Write(mov0CodeGen.GetBufferSpan());
            codeGen.Write(mov1CodeGen.GetBufferSpan());
            return new ExpressionResult(Constants.BoolType, targetRegister);
        }

        private static (Register first, ExpressionResult second, Type type) PrepareBinaryExpression(
            IExpression first,
            IExpression second,
            PreferredRegister preferredFirst,
            CodeGen codeGen,
            StorageManager storageManager,
            IScopeContext scope,
            IAddressFixer addressFixer,
            ICompilationContext context
        )
        {
            var firstResult = context.CompileExpression(first, codeGen, storageManager, preferredFirst, scope);
            var isfirstIntegerType = firstResult.ValueType.IsIntegerRegisterType();
            var firstTemp = storageManager.Allocate(isfirstIntegerType);
            firstTemp.Store(firstResult, codeGen, addressFixer);

            var secondResult = context.CompileExpression(second, codeGen, storageManager, preferredFirst, scope);
            var type = GetOrPromoteToSame(firstResult.ValueType, secondResult.ValueType);
            var preferredRegister = preferredFirst.MakeFor(type);
            var firstRegister = secondResult.IsOccopied(preferredRegister) ? secondResult.GetUnoccupiedVolatile(type) : preferredRegister;
            firstTemp.AsExpressionResult(firstResult.ValueType).GenerateMoveTo(firstRegister, type, codeGen, addressFixer);
            storageManager.Release(firstTemp);
            return (firstRegister, secondResult, type);
        }

        private static Type GetOrPromoteToSame(Type leftType, Type rightType)
        {
            if (leftType.CanAssignImplicitly(rightType))
                return leftType;

            rightType.AssertCanAssignImplicitly(leftType);
            return rightType;
        }
    }
}