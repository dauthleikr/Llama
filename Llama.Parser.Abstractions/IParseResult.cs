namespace Llama.Parser.Abstractions
{
    public interface IParseResult<out T> where T : class, IEntity
    {
        bool Successful { get; }
        IErrorWithConfidence ResultError { get; }
        T ResultSuccess { get; }
    }
}