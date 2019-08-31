namespace Llama.PE.Packaging
{
    internal class RawPackagingResult : IPackagingResult
    {
        public byte[] RawData { get; }

        public RawPackagingResult(byte[] rawData) => RawData = rawData;
    }
}