namespace Llama.PE.Packaging.PE32Plus.OptionalHeader
{
    internal interface IOptionalHeaderResult : IPackagingResult
    {
        bool HasDebugInfo { get; }
        bool HasRelocationInfo { get; }
    }
}