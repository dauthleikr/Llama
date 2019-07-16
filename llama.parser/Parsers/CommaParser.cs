namespace Llama.Parser.Parsers
{
    using Abstractions;
    using Entities;

    internal class CommaParser : AtomicEntityParser<CommaEntity>
    {
        protected override IParseResult<CommaEntity> TryReadEntityInternal(ISourceReader reader, IParseContext context)
        {
            reader.Eat();
            return new CommaEntity();
        }

        public override bool IsPlausible(ISourcePeeker reader, IParseContext context) => reader.Peek() == ',';
    }
}