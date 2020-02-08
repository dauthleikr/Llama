namespace Llama.Compiler.ExpressionCompilers
{
    using System;
    using Parser.Nodes;
    using spit;

    internal class GenericExpressionCompiler : ICompileExpressions<IExpression>
    {
        public ExpressionResult Compile(
            IExpression expression,
            PreferredRegister target,
            CodeGen codeGen,
            StorageManager storageManager,
            ISymbolResolver scope,
            ILinkingInfo linkingInfo,
            ICompilationContext context
        )
        {
            switch (expression)
            {
                case ArrayAccessExpression arrayAccessExpression:
                    return context.CompileExpression(arrayAccessExpression, codeGen, storageManager, target, scope);
                case ArrayAllocationExpression arrayAllocationExpression:
                    return context.CompileExpression(arrayAllocationExpression, codeGen, storageManager, target, scope);
                case AtomicExpression atomicExpression:
                    return context.CompileExpression(atomicExpression, codeGen, storageManager, target, scope);
                case BinaryOperatorExpression binaryOperatorExpression:
                    return context.CompileExpression(binaryOperatorExpression, codeGen, storageManager, target, scope);
                case MethodCallExpression methodCallExpression:
                    return context.CompileExpression(methodCallExpression, codeGen, storageManager, target, scope);
                case TypeCastExpression typeCastExpression:
                    return context.CompileExpression(typeCastExpression, codeGen, storageManager, target, scope);
                case UnaryOperatorExpression unaryOperatorExpression:
                    return context.CompileExpression(unaryOperatorExpression, codeGen, storageManager, target, scope);
                default:
                    throw new NotImplementedException($"Compiler for expression type \"{expression.GetType().Name}\" has not yet been implemented");
            }
        }
    }
}