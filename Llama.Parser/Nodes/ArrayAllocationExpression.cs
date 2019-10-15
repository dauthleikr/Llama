namespace Llama.Parser.Nodes
{
    internal class ArrayAllocationExpression : IExpression
    {
        public Type Type { get; }
        public IExpression Count { get; }

        public ArrayAllocationExpression(Type type, IExpression count)
        {
            Type = type;
            Count = count;
        }
    }
}