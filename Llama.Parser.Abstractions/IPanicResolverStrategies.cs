namespace Llama.Parser.Abstractions
{
    public interface IPanicResolverStrategies
    {
        IPanicResolver GetStrategy<T>();
    }
}