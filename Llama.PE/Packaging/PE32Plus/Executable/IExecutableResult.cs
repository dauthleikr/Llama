namespace Llama.PE.Packaging.PE32Plus.Executable
{
    using System.Collections.Generic;
    using Idata;
    using Structures.Header;

    internal interface IExecutableResult : IPackagingResult
    {
        IResolveIATEntries IATResolver { get; }
        IReadOnlyList<SectionHeader> SectionHeaders { get; }
    }
}