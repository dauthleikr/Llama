namespace Llama.PE.Packaging.PE32Plus
{
    internal interface IMZResult : IPackagingResult
    {
        uint NewHeaderOffset { get; }
    }
}