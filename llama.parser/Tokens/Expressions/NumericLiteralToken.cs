namespace Llama.Parser.Tokens.Expressions
{
    using Language;

    abstract class NumericLiteralToken : AtomicToken<NumericLiteralToken>, IExpressionToken
    {
        public abstract BasicType Type { get; }
    }
}
