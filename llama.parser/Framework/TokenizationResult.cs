namespace Llama.Parser.Framework
{
    public class TokenizationResult<T> : ITokenizationResult<T> where T : class, IToken
    {
        public IErrorWithConfidence ResultError { get; }
        public T ResultSuccess { get; }
        public bool Successful { get; }
        public static readonly TokenizationResult<T> ErrorNotPlausible = new TokenizationResult<T>((IErrorWithConfidence) null);

        public TokenizationResult(T token)
        {
            Successful = true;
            ResultSuccess = token;
        }

        public TokenizationResult(IErrorWithConfidence error)
        {
            Successful = false;
            ResultError = error;
        }

        public static implicit operator TokenizationResult<T>(T token) => new TokenizationResult<T>(token);
    }
}