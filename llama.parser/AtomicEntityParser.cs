namespace Llama.Parser
{
    using Abstractions;
    using Framework;

    public abstract class AtomicEntityParser<T> : ParserBase<T> where T : class, IAtomicEntity
    {
        public override IParseResult<T> TryRead(ISourceReader reader, IParseContext context, INonCodeParser nonCodeParser)
        {
            var preTokenNonCode = nonCodeParser.ReadOrNull(reader);
            var result = TryReadEntityInternal(reader, context);
            if (result.Successful)
            {
                result.ResultSuccess.PreNonCode = preTokenNonCode;
                result.ResultSuccess.PostNonCode = nonCodeParser.ReadOrNull(reader);
            }

            return result;
        }

        protected abstract IParseResult<T> TryReadEntityInternal(ISourceReader reader, IParseContext context);
    }
}