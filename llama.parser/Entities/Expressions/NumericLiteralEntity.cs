namespace Llama.Parser.Entities.Expressions
{
    using Language;

    internal abstract class NumericLiteralEntity : AtomicEntity<NumericLiteralEntity>, IExpressionEntity
    {
        public abstract BasicType Type { get; }
    }
}