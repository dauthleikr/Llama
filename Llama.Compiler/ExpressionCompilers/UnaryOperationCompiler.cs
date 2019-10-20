namespace Llama.Compiler.ExpressionCompilers
{
    using System;
    using Extensions;
    using Parser.Lexer;
    using Parser.Nodes;
    using spit;
    using Type = Parser.Nodes.Type;

    internal class UnaryOperationCompiler : ICompileExpressions<UnaryOperatorExpression>
    {
        public ExpressionResult Compile(
            UnaryOperatorExpression expression,
            PreferredRegister target,
            CodeGen codeGen,
            StorageManager storageManager,
            IScopeContext scope,
            IAddressFixer addressFixer,
            ICompilationContext context
        )
        {
            return expression.Operator.Operator.Kind switch
            {
                TokenKind.Minus     => CompileMinus(expression, target, codeGen, context, storageManager, addressFixer, scope),
                TokenKind.AddressOf => CompileAddressOf(expression, target, codeGen, context, storageManager, addressFixer, scope),
                TokenKind.Not       => CompileNot(expression, target, codeGen, context, storageManager, addressFixer, scope),
                _ => throw new NotImplementedException(
                    $"{nameof(UnaryOperationCompiler)}: I do not know how to compile {expression.Operator.Operator}"
                )
            };
        }

        private static ExpressionResult CompileNot(
            UnaryOperatorExpression expression,
            PreferredRegister target,
            CodeGen codeGen,
            ICompilationContext context,
            StorageManager storageManager,
            IAddressFixer addressFixer,
            IScopeContext scope
        )
        {
            var result = context.CompileExpression(expression.Expression, codeGen, storageManager, target, scope);
            Constants.BoolType.AssertCanAssign(result.ValueType);

            var register = target.MakeFor(Constants.BoolType);
            result.GenerateMoveTo(register, codeGen, addressFixer);
            codeGen.Xor(register.AsR8(), 1);
            return new ExpressionResult(Constants.BoolType, register);
        }

        private static ExpressionResult CompileAddressOf(
            UnaryOperatorExpression expression,
            PreferredRegister target,
            CodeGen codeGen,
            ICompilationContext context,
            StorageManager storageManager,
            IAddressFixer addressFixer,
            IScopeContext scope
        )
        {
            var result = context.CompileExpression(expression.Expression, codeGen, storageManager, target, scope);
            if (result.Kind == ExpressionResult.ResultKind.Value)
                throw new ReferenceException($"Can not take the address of the r-value of: {expression.Expression}");
            var type = new Type(result.ValueType, Type.WrappingType.PointerOf);
            var register = target.MakeFor(type);

            result.LeaTo(register, codeGen, addressFixer);
            return new ExpressionResult(type, register);
        }

        private static ExpressionResult CompileMinus(
            UnaryOperatorExpression expression,
            PreferredRegister target,
            CodeGen codeGen,
            ICompilationContext context,
            StorageManager storageManager,
            IAddressFixer addressFixer,
            IScopeContext scope
        )
        {
            var result = context.CompileExpression(expression.Expression, codeGen, storageManager, target, scope);

            if (!result.ValueType.IsIntegerRegisterType()) // todo: float negation
            {
                throw new NotImplementedException(
                    $"{nameof(UnaryOperationCompiler)}: {nameof(CompileMinus)}: Is not implemented for type: {result.ValueType}"
                );
            }

            var register = target.MakeFor(result.ValueType);
            result.GenerateMoveTo(register, codeGen, addressFixer);
            codeGen.Neg(register);
            return new ExpressionResult(result.ValueType, register);
        }
    }
}