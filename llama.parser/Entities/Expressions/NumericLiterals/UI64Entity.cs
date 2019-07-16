// ReSharper disable InconsistentNaming

namespace Llama.Parser.Entities.Expressions.NumericLiterals
{
    using Language;

    internal class UI64Entity : NumericLiteralEntity
    {
        public override BasicType Type => BasicType.UI64;
        private readonly string _token;
        public readonly ulong Integer;

        public UI64Entity(string token, ulong integer)
        {
            _token = token;
            Integer = integer;
        }

        protected override string ToStringInternal() => _token;
    }
}