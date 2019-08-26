namespace Llama.PE.Packaging.PE32Plus.Idata
{
    using Sections;

    internal interface IIdataResult : IPackagingResult, ISectionInfo
    {
        uint ImportDirectoryTableRVA { get; }
        uint IAT_RVA { get; }
        uint IdataRVA { get; }
        IResolveIATEntries IATResolver { get; }
    }
}