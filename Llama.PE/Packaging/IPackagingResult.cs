namespace Llama.PE.Packaging
{
    using System;

    internal interface IPackagingResult
    {
        byte[] RawData { get; }
    }
}