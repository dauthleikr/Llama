namespace Llama.Parser.Tokens
{
    public class CommaToken : AtomicToken<CommaToken>
    {
        protected override string ToStringInternal() => ",";
    }
}