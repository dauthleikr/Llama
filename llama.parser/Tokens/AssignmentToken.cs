namespace Llama.Parser.Tokens
{
    public class AssignmentToken : AtomicToken<AssignmentToken>
    {
        protected override string ToStringInternal() => "=";
    }
}