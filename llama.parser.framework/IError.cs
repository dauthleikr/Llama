namespace Llama.Parser.Framework
{
    public interface IError
    {
        string Message { get; }
        long Index { get; }
        int Length { get; }
    }
}