namespace Llama.Parser.Nodes
{
    internal class TypeCast : IExpression
    {
        public Type Type { get; }
        public IExpression CastExpression { get; }

        public TypeCast(Type type, IExpression castExpression)
        {
            Type = type;
            CastExpression = castExpression;
        }
    }
}