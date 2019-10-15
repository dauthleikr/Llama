namespace Llama.Parser.Nodes
{
    public class TypeCastExpression : IExpression
    {
        public Type Type { get; }
        public IExpression CastExpression { get; }

        public TypeCastExpression(Type type, IExpression castExpression)
        {
            Type = type;
            CastExpression = castExpression;
        }
    }
}