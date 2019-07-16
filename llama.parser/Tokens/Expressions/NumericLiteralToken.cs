namespace Llama.Parser.Tokens.Expressions
{
    using Language;

    internal abstract class NumericLiteralToken : AtomicToken<NumericLiteralToken>, IExpressionToken
    {
        public abstract BasicType Type { get; }
    }
}