namespace Llama.Parser.Nodes
{
    public class BinaryOperatorExpression : IExpression
    {
        public IExpression Left { get; internal set; }
        public BinaryOperator Operator { get; }
        public IExpression Right { get; }

        public BinaryOperatorExpression(IExpression left, BinaryOperator @operator, IExpression right)
        {
            Left = left;
            Operator = @operator;
            Right = right;
        }
    }
}