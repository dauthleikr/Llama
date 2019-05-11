namespace Llama.Parser.Framework
{
    public interface IParseStrategies
    {
        IParse<T> GetStrategyFor<T>() where T : class, IToken;
    }
}