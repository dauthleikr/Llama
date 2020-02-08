namespace Llama.Parser.Nodes
{
    public class FunctionCallExpression : IExpression
    {
        public IExpression Expression { get; }
        public IExpression[] Parameters { get; }

        public FunctionCallExpression(IExpression expression, params IExpression[] parameters)
        {
            Expression = expression;
            Parameters = parameters;
        }
    }
}