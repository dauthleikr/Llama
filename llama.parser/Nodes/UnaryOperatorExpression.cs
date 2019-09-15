namespace Llama.Parser.Nodes
{
    internal class UnaryOperatorExpression : IExpression
    {
        public UnaryOperator Operator { get; }
        public IExpression Expression { get; }

        public UnaryOperatorExpression(UnaryOperator @operator, IExpression expression)
        {
            Operator = @operator;
            Expression = expression;
        }
    }
}