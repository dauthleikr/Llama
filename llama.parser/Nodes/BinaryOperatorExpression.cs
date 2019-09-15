namespace Llama.Parser.Nodes
{
    internal class BinaryOperatorExpression
    {
        public IExpression Left { get; internal set; }
        public BinaryOperator Operator { get; }
        public IExpression Right { get; internal set; }

        public BinaryOperatorExpression(IExpression left, BinaryOperator @operator, IExpression right)
        {
            Left = left;
            Operator = @operator;
            Right = right;
        }
    }
}