namespace Llama.Parser.Abstractions
{
    public interface IError
    {
        string Message { get; }
        long Index { get; }
        int Length { get; }
    }
}