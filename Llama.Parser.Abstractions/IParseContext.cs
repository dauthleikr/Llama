namespace Llama.Parser.Abstractions
{
    public interface IParseContext
    {
        bool TryRead<T>(out T result) where T : class, IEntity;
        IParseResult<T> TryRead<T>() where T : class, IEntity;
        bool IsPlausible<T>() where T : class, IEntity;
        void Panic<T>() where T : IPanicResolver;
    }
}