namespace Llama.Parser.Tokens.Expressions.NumericLiterals
{
    using Language;

    internal class DoubleToken : NumericLiteralToken
    {
        private readonly string _token;
        public readonly double Number;

        public DoubleToken(string token, double number)
        {
            _token = token;
            Number = number;
        }


        protected override string ToStringInternal() => _token;

        public override BasicType Type => BasicType.Double;
    }
}