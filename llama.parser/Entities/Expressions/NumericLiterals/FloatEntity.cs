namespace Llama.Parser.Entities.Expressions.NumericLiterals
{
    using Language;

    internal class FloatEntity : NumericLiteralEntity
    {
        public override BasicType Type => BasicType.Float;
        private readonly string _token;
        public readonly float Number;

        public FloatEntity(string token, float number)
        {
            _token = token;
            Number = number;
        }

        protected override string ToStringInternal() => _token;
    }
}