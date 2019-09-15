namespace Llama.Parser.Nodes
{
    internal class TypeCast : IExpression
    {
        public TypeNode Type { get; }
        public IExpression CastExpression { get; }

        public TypeCast(TypeNode type, IExpression castExpression)
        {
            Type = type;
            CastExpression = castExpression;
        }
    }
}