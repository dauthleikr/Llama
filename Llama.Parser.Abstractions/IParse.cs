namespace Llama.Parser.Abstractions
{
    public interface IParse<out T> where T : class, IEntity
    {
        IParseResult<T> TryRead(ISourceReader reader, IParseContext context, INonCodeParser nonCodeParser);
        bool IsPlausible(ISourcePeeker reader, IParseContext context);
    }
}