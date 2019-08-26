namespace Llama.PE.Packaging.PE32Plus.Sections
{
    using System.Collections.Generic;
    using Structures.Header;

    internal interface ISectionsResult : IPackagingResult
    {
        IReadOnlyList<SectionHeader> SectionHeaders { get; }
    }
}