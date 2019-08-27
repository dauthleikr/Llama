namespace Llama.PE.Packaging.PE32Plus.Idata
{
    using Sections;
    using Structures.Header;

    internal interface IIdataResult : IPackagingResult, ISectionInfo
    {
        ImageDataDirectory ImportDirectory { get; }
        ImageDataDirectory IAT { get; }
        uint IdataRVA { get; }
        IResolveIATEntries IATResolver { get; }
    }
}