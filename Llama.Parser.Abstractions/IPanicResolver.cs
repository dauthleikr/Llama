namespace Llama.Parser.Framework
{
    public interface IPanicResolver
    {
        int GetFaultyCodeLength(ISourcePeeker reader);
    }
}