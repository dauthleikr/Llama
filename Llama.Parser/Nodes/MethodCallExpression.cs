namespace Llama.Parser.Nodes
{
    public class MethodCallExpression : IExpression
    {
        public IExpression Expression { get; }
        public IExpression[] Parameters { get; }

        public MethodCallExpression(IExpression expression, params IExpression[] parameters)
        {
            Expression = expression;
            Parameters = parameters;
        }
    }
}