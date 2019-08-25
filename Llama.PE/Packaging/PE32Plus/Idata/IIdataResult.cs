namespace Llama.PE.Packaging.PE32Plus.Idata
{
    internal interface IIdataResult : IPackagingResult
    {
        uint ImportDirectoryTableRVA { get; }
        uint IAT_RVA { get; }
        uint IdataRVA { get; }
        uint GetRVAOfIATEntry(string library, string function);
    }
}