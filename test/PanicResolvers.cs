namespace test
{
    using System.Diagnostics;
    using Llama.Parser.Abstractions;

    internal class PanicResolvers : IPanicResolverStrategies, IPanicResolver
    {
        public int GetFaultyCodeLength(ISourcePeeker reader)
        {
            Debug.WriteLine($"PANIC: {reader.Position}");
            return 1;
        }

        public IPanicResolver GetStrategy<T>() => this;
    }
}