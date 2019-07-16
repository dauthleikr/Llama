namespace Llama.Parser.Entities.Expressions.NumericLiterals
{
    using Language;

    internal class DoubleEntity : NumericLiteralEntity
    {
        public override BasicType Type => BasicType.Double;
        private readonly string _token;
        public readonly double Number;

        public DoubleEntity(string token, double number)
        {
            _token = token;
            Number = number;
        }


        protected override string ToStringInternal() => _token;
    }
}