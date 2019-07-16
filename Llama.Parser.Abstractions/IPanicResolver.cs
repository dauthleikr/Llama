namespace Llama.Parser.Abstractions
{
    public interface IPanicResolver
    {
        int GetFaultyCodeLength(ISourcePeeker reader);
    }
}