namespace Llama.Parser.Framework
{
    public interface IParseContext
    {
        bool TryReadToken<T>(out T result) where T : class, IToken;
        ITokenizationResult<T> TryReadToken<T>() where T : class, IToken;
        bool IsPlausible<T>() where T : class, IToken;
        void Panic<T>() where T : IPanicResolver;
    }
}