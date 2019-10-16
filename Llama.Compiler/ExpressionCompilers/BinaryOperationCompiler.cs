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
                    return CompileAdd(expression.Left, expression.Right, target, codeGen, storageManager, scope, addressFixer, context);
                case TokenKind.Minus:
                    return CompileSubtract(expression.Left, expression.Right, target, codeGen, storageManager, scope, addressFixer, context);
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
                        codeGen.Je,
                        true
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
                        codeGen.Jbe,
                        true
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
                        codeGen.Jb,
                        true
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
                        codeGen.Jbe
                    );
                default:
                    throw new NotImplementedException($"Compilation for Operator {expression.Operator.Operator.Kind} is not implemented");
            }
        }

        private static ExpressionResult CompileAdd(
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
            var (leftReg, rightExpr, type) = PrepareBinaryExpression(left, right, target, codeGen, storageManager, scope, addressFixer, context);

            if (type.IsIntegerRegisterType())
                rightExpr.AddTo(leftReg, codeGen, addressFixer);
            else if (type == Constants.DoubleType)
                rightExpr.AddsdTo(leftReg, codeGen, addressFixer);
            else if (type == Constants.FloatType)
                rightExpr.AddssTo(leftReg, codeGen, addressFixer);
            else
            {
                throw new NotImplementedException(
                    $"{nameof(BinaryOperationCompiler)}: {nameof(CompileAdd)}: I do not know how to compile this type: {type}"
                );
            }

            return new ExpressionResult(type, leftReg);
        }

        private static ExpressionResult CompileSubtract(
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
            var (leftReg, rightExpr, type) = PrepareBinaryExpression(left, right, target, codeGen, storageManager, scope, addressFixer, context);

            if (type.IsIntegerRegisterType())
                rightExpr.SubTo(leftReg, codeGen, addressFixer);
            else if (type == Constants.DoubleType)
                rightExpr.SubsdTo(leftReg, codeGen, addressFixer);
            else if (type == Constants.FloatType)
                rightExpr.SubssTo(leftReg, codeGen, addressFixer);
            else
            {
                throw new NotImplementedException(
                    $"{nameof(BinaryOperationCompiler)}: {nameof(CompileSubtract)}: I do not know how to compile this type: {type}"
                );
            }

            return new ExpressionResult(type, leftReg);
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
            Action<sbyte> comparisonJmp,
            bool inverted = false
        )
        {
            var (leftReg, rightExpr, type) = PrepareBinaryExpression(left, right, target, codeGen, storageManager, scope, addressFixer, context);
            if (type.IsIntegerRegisterType())
                rightExpr.TestTo(leftReg, codeGen, addressFixer);
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
            mov0CodeGen.Jmp(mov1CodeGen.GetDataSpan().Length);

            comparisonJmp((sbyte)mov0CodeGen.GetDataSpan().Length);
            codeGen.Write(mov0CodeGen.GetDataSpan());
            codeGen.Write(mov1CodeGen.GetDataSpan());
            return new ExpressionResult(type, targetRegister);
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
            var firstResult = context.CompileExpression(first, codeGen, storageManager, preferredFirst, scope); // todo: compile into temp storage
            var isfirstIntegerType = firstResult.ValueType.IsIntegerRegisterType();
            var firstTemp = storageManager.Allocate(isfirstIntegerType);
            firstTemp.Store(firstResult, codeGen, addressFixer);
            
            var secondResult = context.CompileExpression(second, codeGen, storageManager, preferredFirst, scope);
            var type = GetOrPromoteToSame(firstResult.ValueType, secondResult.ValueType);
            var preferredRegister = preferredFirst.MakeFor(type);
            var firstRegister = secondResult.IsOccopied(preferredRegister) ?
                secondResult.GetUnoccupiedVolatile(firstResult.ValueType) :
                preferredRegister;
            firstTemp.AsExpressionResult(firstResult.ValueType).GenerateMoveTo(firstRegister, type, codeGen, addressFixer);
            storageManager.Release(firstTemp);
            return (firstRegister, secondResult, type);
        }

        private static Type GetOrPromoteToSame(Type leftType, Type rightType)
        {
            if (leftType.CanAssign(rightType))
                return leftType;

            rightType.AssertCanAssign(leftType);
            return rightType;
        }
    }
}