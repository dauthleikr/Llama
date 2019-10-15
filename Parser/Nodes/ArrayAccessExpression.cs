namespace Llama.Parser.Nodes
{
    internal class ArrayAccessExpression : IExpression
    {
        public IExpression Array { get; }
        public IExpression Index { get; }

        public ArrayAccessExpression(IExpression array, IExpression index)
        {
            Array = array;
            Index = index;
        }
    }
}