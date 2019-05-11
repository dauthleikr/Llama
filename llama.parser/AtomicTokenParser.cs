namespace Llama.Parser
{
    using Framework;

    internal abstract class AtomicTokenParser<T> : ParserBase<T> where T : class, IAtomicToken
    {
        public override ITokenizationResult<T> TryReadToken(ISourceReader reader, IParseContext context, INonCodeParser nonCodeParser)
        {
            var preTokenNonCode = nonCodeParser.ReadOrNull(reader);
            var tokenResult = TryReadTokenInternal(reader, context);
            if (tokenResult.Successful)
            {
                tokenResult.ResultSuccess.PreNonCode = preTokenNonCode;
                tokenResult.ResultSuccess.PostNonCode = nonCodeParser.ReadOrNull(reader);
            }

            return tokenResult;
        }

        protected abstract ITokenizationResult<T> TryReadTokenInternal(ISourceReader reader, IParseContext context);
    }
}