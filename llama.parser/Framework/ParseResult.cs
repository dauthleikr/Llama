namespace Llama.Parser.Framework
{
    using Abstractions;

    public class ParseResult<T> : IParseResult<T> where T : class, IEntity
    {
        public IErrorWithConfidence ResultError { get; }
        public T ResultSuccess { get; }
        public bool Successful { get; }
        public static readonly ParseResult<T> ErrorNotPlausible = new ParseResult<T>((IErrorWithConfidence) null);

        public ParseResult(T token)
        {
            Successful = true;
            ResultSuccess = token;
        }

        public ParseResult(IErrorWithConfidence error)
        {
            Successful = false;
            ResultError = error;
        }

        public static implicit operator ParseResult<T>(T token) => new ParseResult<T>(token);
    }
}