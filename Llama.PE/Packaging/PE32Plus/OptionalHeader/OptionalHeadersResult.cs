namespace Llama.PE.Packaging.PE32Plus.OptionalHeader
{
    internal class OptionalHeadersResult : IOptionalHeaderResult
    {
        public byte[] RawData { get; }

        public OptionalHeadersResult(byte[] rawData) => RawData = rawData;
    }
}