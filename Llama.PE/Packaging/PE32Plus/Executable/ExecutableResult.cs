namespace Llama.PE.Packaging.PE32Plus.Executable
{
    using System.Collections.Generic;
    using Idata;
    using Structures.Header;

    internal class ExecutableResult : IExecutableResult
    {
        public byte[] RawData { get; }
        public IResolveIATEntries IATResolver { get; }
        public IReadOnlyList<SectionHeader> SectionHeaders { get; }

        public ExecutableResult(byte[] rawData, IResolveIATEntries iatResolver, IReadOnlyList<SectionHeader> sectionHeaders)
        {
            RawData = rawData;
            IATResolver = iatResolver;
            SectionHeaders = sectionHeaders;
        }
    }
}