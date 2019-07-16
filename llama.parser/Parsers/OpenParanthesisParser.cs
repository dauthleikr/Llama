namespace Llama.Parser.Parsers
{
    using Abstractions;
    using Entities;

    public class OpenParanthesisParser : AtomicEntityParser<OpenParanthesisEntity>
    {
        protected override IParseResult<OpenParanthesisEntity> TryReadEntityInternal(ISourceReader reader, IParseContext context)
        {
            reader.Eat();
            return new OpenParanthesisEntity();
        }

        public override bool IsPlausible(ISourcePeeker reader, IParseContext context) => reader.Peek() == '(';
    }
}