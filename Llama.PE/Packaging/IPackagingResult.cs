namespace Llama.PE.Packaging
{
    using System;

    internal interface IPackagingResult
    {
        ReadOnlySpan<byte> RawData { get; }
    }
}