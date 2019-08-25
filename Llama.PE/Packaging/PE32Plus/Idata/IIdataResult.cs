namespace Llama.PE.Packaging.PE32Plus.Idata
{
    internal interface IIdataResult : IPackagingResult
    {
        uint GetRVAOfIATEntry(string library, string function);
    }
}