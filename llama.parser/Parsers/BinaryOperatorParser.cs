namespace Llama.Parser.Parsers
{
    using System.Collections.Generic;
    using System.Linq;
    using Framework;
    using Language;
    using Tokens;

    internal class BinaryOperatorParser : AtomicTokenParser<BinaryOperatorToken>
    {
        private static readonly HashSet<string> Operators = BinaryOperators.OperatorToEnum.Keys.ToHashSet();
        private static readonly HashSet<char> ValidFirstOperatorChars = Operators.Select(item => item.First()).Distinct().ToHashSet();
        private static readonly int MaxOperatorLength = Operators.Max(k => k.Length);

        protected override ITokenizationResult<BinaryOperatorToken> TryReadTokenInternal(ISourceReader reader, IParseContext context)
        {
            var operatorString = reader.TryReadLongest(Operators, MaxOperatorLength);
            if (operatorString == null)
                return ErrorExpectedToken(reader);
            return new BinaryOperatorToken(BinaryOperators.OperatorToEnum[operatorString]);
        }

        public override bool IsPlausible(ISourcePeeker reader, IParseContext context) => ValidFirstOperatorChars.Contains(reader.Peek());
    }
}