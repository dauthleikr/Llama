namespace Llama.Parser.Framework
{
    public interface IPanicResolverStrategies
    {
        IPanicResolver GetStrategy<T>();
    }
}