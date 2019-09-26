namespace Llama.Compiler.ExpressionCompilers
{
    using Parser.Nodes;
    using spit;

    internal class GenericExpressionCompiler : ICompileExpressions<IExpression>
    {
        public void Compile(
            IExpression expression,
            Register target,
            CodeGen codeGen,
            IFunctionContext function,
            IAddressFixer addressFixer,
            ICompilationContext context
        )
        {
            switch (expression)
            {
                case ArrayAccessExpression arrayAccessExpression:
                    break;
                case ArrayAllocationExpression arrayAllocationExpression:
                    break;
                case AtomicExpression atomicExpression:
                    break;
                case BinaryOperatorExpression binaryOperatorExpression:
                    break;
                case MethodCallExpression methodCallExpression:
                    break;
                case TypeCastExpression typeCastExpression:
                    break;
                case UnaryOperatorExpression unaryOperatorExpression:
                    break;
            }
        }
    }
}