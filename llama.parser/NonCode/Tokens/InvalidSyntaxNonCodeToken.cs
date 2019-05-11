namespace Llama.Parser.NonCode.Tokens
{
    public class InvalidSyntaxNonCodeToken : AtomicNonCodeToken
    {
        public readonly string InvalidCode;

        public InvalidSyntaxNonCodeToken(string invalidCode) => InvalidCode = invalidCode;

        public override string ToString() => InvalidCode;
    }
}