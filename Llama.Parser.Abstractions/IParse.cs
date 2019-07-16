namespace Llama.Parser.Framework
{
    public interface IParse<out T> where T : class, IToken
    {
        ITokenizationResult<T> TryReadToken(ISourceReader reader, IParseContext context, INonCodeParser nonCodeParser);
        bool IsPlausible(ISourcePeeker reader, IParseContext context);
    }
}