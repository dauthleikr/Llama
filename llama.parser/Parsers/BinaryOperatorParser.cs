namespace Llama.Parser.Parsers
{
    using System.Collections.Generic;
    using System.Linq;
    using Abstractions;
    using Entities;
    using Framework;
    using Language;

    public class BinaryOperatorParser : AtomicEntityParser<BinaryOperatorEntity>
    {
        private static readonly HashSet<string> Operators = BinaryOperators.OperatorToEnum.Keys.ToHashSet();
        private static readonly HashSet<char> ValidFirstOperatorChars = Operators.Select(item => item.First()).Distinct().ToHashSet();
        private static readonly int MaxOperatorLength = Operators.Max(k => k.Length);

        protected override IParseResult<BinaryOperatorEntity> TryReadEntityInternal(ISourceReader reader, IParseContext context)
        {
            var operatorString = reader.TryReadLongest(Operators, MaxOperatorLength);
            if (operatorString == null)
                return ErrorExpectedEntity(reader);
            return new BinaryOperatorEntity(BinaryOperators.OperatorToEnum[operatorString]);
        }

        public override bool IsPlausible(ISourcePeeker reader, IParseContext context) => ValidFirstOperatorChars.Contains(reader.Peek());
    }
}