namespace Llama.Parser.Tokens
{
    public class OpenParanthesisToken : AtomicToken<OpenParanthesisToken>
    {
        protected override string ToStringInternal() => "(";
    }
}