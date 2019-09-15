namespace Llama.Parser.Abstractions
{
    public interface IParseStore
    {
        IParse<T> GetStrategyFor<T>();
    }
}