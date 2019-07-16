namespace Llama.Parser.Abstractions
{
    public interface INonCodeParser
    {
        INonCode ReadOrNull(ISourceReader reader);
        void MarkAsNonCode(long position, int length);
    }
}