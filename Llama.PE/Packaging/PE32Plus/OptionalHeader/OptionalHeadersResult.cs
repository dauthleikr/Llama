namespace Llama.PE.Packaging.PE32Plus.OptionalHeader
{
    internal class OptionalHeadersResult : IOptionalHeaderResult
    {
        public byte[] RawData { get; }
        public bool HasDebugInfo { get; }
        public bool HasRelocationInfo { get; }

        public OptionalHeadersResult(byte[] rawData, bool hasDebugInfo, bool hasRelocationInfo)
        {
            RawData = rawData;
            HasDebugInfo = hasDebugInfo;
            HasRelocationInfo = hasRelocationInfo;
        }
    }
}