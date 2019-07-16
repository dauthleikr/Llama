namespace Llama.Parser.Abstractions
{
    public interface IParseStrategies
    {
        IParse<T> GetStrategyFor<T>() where T : class, IEntity;
    }
}