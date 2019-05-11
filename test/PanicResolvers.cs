using System;
using System.Collections.Generic;
using System.Text;

namespace test
{
    using System.Diagnostics;
    using Llama.Parser.Framework;

    class PanicResolvers : IPanicResolverStrategies, IPanicResolver
    {
        public IPanicResolver GetStrategy<T>() => this;

        public int GetFaultyCodeLength(ISourcePeeker reader)
        {
            Debug.WriteLine($"PANIC: {reader.Position}");
            return 1;
        }
    }
}
