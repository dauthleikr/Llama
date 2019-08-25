namespace Llama.PE.Packaging.PE32Plus.MZHeader
{
    internal interface IMZResult : IPackagingResult
    {
        uint NewHeaderOffset { get; }
    }
}