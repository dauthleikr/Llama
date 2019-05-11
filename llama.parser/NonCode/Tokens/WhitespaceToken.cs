namespace Llama.Parser.NonCode.Tokens
{
    public class WhitespaceToken : AtomicNonCodeToken
    {
        public readonly string Whitespaces;

        public WhitespaceToken(string whitespaces) => Whitespaces = whitespaces;

        public override string ToString() => Whitespaces;
    }
}
