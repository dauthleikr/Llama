namespace Llama.Parser.Entities.Expressions.NumericLiterals
{
    using Language;

    internal class I64Entity : NumericLiteralEntity
    {
        public override BasicType Type => BasicType.I64;
        private readonly string _token;
        public readonly long Integer;

        public I64Entity(string token, long integer)
        {
            _token = token;
            Integer = integer;
        }

        protected override string ToStringInternal() => _token;
    }
}