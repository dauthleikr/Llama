namespace Llama.Parser
{
    public interface IParseStore
    {
        IParse<T> GetStrategyFor<T>();
    }
}