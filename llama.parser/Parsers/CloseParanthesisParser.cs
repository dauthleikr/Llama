namespace Llama.Parser.Parsers
{
    using Abstractions;
    using Entities;

    internal class CloseParanthesisParser : AtomicEntityParser<CloseParanthesisEntity>
    {
        protected override IParseResult<CloseParanthesisEntity> TryReadEntityInternal(ISourceReader reader, IParseContext context)
        {
            reader.Eat();
            return new CloseParanthesisEntity();
        }

        public override bool IsPlausible(ISourcePeeker reader, IParseContext context) => reader.Peek() == ')';
    }
}