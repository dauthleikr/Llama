namespace Llama.Parser.Tokens.Expressions.NumericLiterals
{
    using Language;

    internal class I64Token : NumericLiteralToken
    {
        public override BasicType Type => BasicType.I64;
        private readonly string _token;
        public readonly long Integer;

        public I64Token(string token, long integer)
        {
            _token = token;
            Integer = integer;
        }

        protected override string ToStringInternal() => _token;
    }
}