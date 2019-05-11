namespace Llama.Parser.Tokens.Expressions.NumericLiterals
{
    using Language;

    internal class UI64Token : NumericLiteralToken
    {
        private readonly string _token;
        public readonly ulong Integer;

        public UI64Token(string token, ulong integer)
        {
            _token = token;
            Integer = integer;
        }

        protected override string ToStringInternal() => _token;
        public override BasicType Type => BasicType.UI64;
    }
}