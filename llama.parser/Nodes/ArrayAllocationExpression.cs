namespace Llama.Parser.Nodes
{
    internal class ArrayAllocationExpression : IExpression
    {
        public TypeNode Type { get; }
        public IExpression Count { get; }

        public ArrayAllocationExpression(TypeNode type, IExpression count)
        {
            Type = type;
            Count = count;
        }
    }
}