namespace Llama.PE.Packaging.PE32Plus.Idata
{
    internal interface IResolveIATEntries
    {
        uint GetRVAOfIATEntry(string library, string function);
    }
}