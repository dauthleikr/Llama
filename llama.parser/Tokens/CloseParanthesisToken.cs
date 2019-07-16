namespace Llama.Parser.Tokens
{
    public class CloseParanthesisToken : AtomicToken<CloseParanthesisToken>
    {
        protected override string ToStringInternal() => ")";
    }
}