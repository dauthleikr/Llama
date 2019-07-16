namespace Llama.Parser.Entities.Expressions
{
    using Language;

    public abstract class NumericLiteralEntity : AtomicEntity<NumericLiteralEntity>, IExpressionEntity
    {
        public abstract BasicType Type { get; }
    }
}