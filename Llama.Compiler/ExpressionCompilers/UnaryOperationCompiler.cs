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
            ISymbolResolver scope,
            ILinkingInfo linkingInfo,
            ICompilationContext context
        )
        {
            return expression.Operator.Operator.Kind switch
            {
                TokenKind.Minus => CompileMinus(expression, target, codeGen, context, storageManager, linkingInfo, scope),
                TokenKind.AddressOf => CompileAddressOf(expression, target, codeGen, context, storageManager, linkingInfo, scope),
                TokenKind.Not => CompileNot(expression, target, codeGen, context, storageManager, linkingInfo, scope),
                TokenKind.Pointer => CompileDereference(expression, target, codeGen, context, storageManager, linkingInfo, scope),
                _ => throw new NotImplementedException(
                    $"{nameof(UnaryOperationCompiler)}: I do not know how to compile {expression.Operator.Operator}"
                )
            };
        }

        private ExpressionResult CompileDereference(UnaryOperatorExpression expression, PreferredRegister target, CodeGen codeGen, ICompilationContext context, StorageManager storageManager, ILinkingInfo linkingInfo, ISymbolResolver scope)
        {
            var result = context.CompileExpression(expression.Expression, codeGen, storageManager, target, scope);
            if (result.ValueType.ChildRelation != Type.WrappingType.PointerOf)
                throw new TypeMismatchException($"Any dereference-able type", result.ValueType.ToString());

            var targetRegister = target.MakeFor(result.ValueType);
            result.GenerateMoveTo(targetRegister, codeGen, linkingInfo);
            return new ExpressionResult(result.ValueType.Child, targetRegister.AsR64(), 0);
        }

        private static ExpressionResult CompileNot(
            UnaryOperatorExpression expression,
            PreferredRegister target,
            CodeGen codeGen,
            ICompilationContext context,
            StorageManager storageManager,
            ILinkingInfo linkingInfo,
            ISymbolResolver scope
        )
        {
            var result = context.CompileExpression(expression.Expression, codeGen, storageManager, target, scope);
            Constants.BoolType.AssertCanAssignImplicitly(result.ValueType);

            var register = target.MakeFor(Constants.BoolType);
            result.GenerateMoveTo(register, codeGen, linkingInfo);
            codeGen.Xor(register.AsR8(), 1);
            return new ExpressionResult(Constants.BoolType, register);
        }

        private static ExpressionResult CompileAddressOf(
            UnaryOperatorExpression expression,
            PreferredRegister target,
            CodeGen codeGen,
            ICompilationContext context,
            StorageManager storageManager,
            ILinkingInfo linkingInfo,
            ISymbolResolver scope
        )
        {
            var result = context.CompileExpression(expression.Expression, codeGen, storageManager, target, scope);
            if (result.Kind == ExpressionResult.ResultKind.Value)
                throw new ReferenceException($"Can not take the address of the r-value of: {expression.Expression}");
            var type = new Type(result.ValueType, Type.WrappingType.PointerOf);
            var register = target.MakeFor(type);

            result.LeaTo(register, codeGen, linkingInfo);
            return new ExpressionResult(type, register);
        }

        private static ExpressionResult CompileMinus(
            UnaryOperatorExpression expression,
            PreferredRegister target,
            CodeGen codeGen,
            ICompilationContext context,
            StorageManager storageManager,
            ILinkingInfo linkingInfo,
            ISymbolResolver scope
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
            result.GenerateMoveTo(register, codeGen, linkingInfo);
            codeGen.Neg(register);
            return new ExpressionResult(result.ValueType, register);
        }
    }
}