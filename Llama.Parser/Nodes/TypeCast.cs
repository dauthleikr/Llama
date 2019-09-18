namespace Llama.Parser.Nodes
{
    public class TypeCast : IExpression
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