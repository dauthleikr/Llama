namespace Llama.Parser.NonCode
{
    using Framework;

    public interface IParseNonCode
    {
        bool TryParse(ISourceReader reader, out INonCode nonCode);
    }

    internal interface IParseNonCode<T> where T : class, INonCode
    {
        bool TryParse(ISourceReader reader, out T nonCode);
    }
}