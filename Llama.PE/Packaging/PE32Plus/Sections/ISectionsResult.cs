namespace Llama.PE.Packaging.PE32Plus.Sections
{
    using System.Collections.Generic;
    using Idata;
    using Structures.Header;

    internal interface ISectionsResult : IPackagingResult
    {
        IReadOnlyList<SectionHeader> SectionHeaders { get; }
        uint EntryPointRVA { get; }
        IResolveIATEntries IATResolver { get; }
    }
}