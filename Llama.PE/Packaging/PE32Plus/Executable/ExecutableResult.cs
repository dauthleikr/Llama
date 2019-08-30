namespace Llama.PE.Packaging.PE32Plus.Executable
{
    using System;

    internal class ExecutableResult : IPackagingResult
    {
        public byte[] RawData { get; }

        public ExecutableResult(byte[] rawData) => RawData = rawData ?? throw new ArgumentNullException(nameof(rawData));
    }
}