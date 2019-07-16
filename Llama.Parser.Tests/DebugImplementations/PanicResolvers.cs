namespace Llama.Parser.Tests.DebugImplementations
{
    using System.Diagnostics;
    using Framework;

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