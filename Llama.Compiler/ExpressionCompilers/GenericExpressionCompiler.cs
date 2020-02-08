namespace Llama.Compiler.ExpressionCompilers
{
    using System;
    using Parser.Nodes;

    internal class GenericExpressionCompiler : ICompileExpressions<IExpression>
    {
        public ExpressionResult Compile(
            IExpression expression,
            PreferredRegister target,
            ICompilationContext context
        )
        {
            switch (expression)
            {
                case ArrayAccessExpression arrayAccessExpression:
                    return context.CompileExpression(arrayAccessExpression, target);
                case ArrayAllocationExpression arrayAllocationExpression:
                    return context.CompileExpression(arrayAllocationExpression, target);
                case AtomicExpression atomicExpression:
                    return context.CompileExpression(atomicExpression, target);
                case BinaryOperatorExpression binaryOperatorExpression:
                    return context.CompileExpression(binaryOperatorExpression, target);
                case FunctionCallExpression methodCallExpression:
                    return context.CompileExpression(methodCallExpression, target);
                case TypeCastExpression typeCastExpression:
                    return context.CompileExpression(typeCastExpression, target);
                case UnaryOperatorExpression unaryOperatorExpression:
                    return context.CompileExpression(unaryOperatorExpression, target);
                default:
                    throw new NotImplementedException($"Compiler for expression type \"{expression.GetType().Name}\" has not yet been implemented");
            }
        }
    }
}