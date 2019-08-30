namespace Llama.PE.Packaging.PE32Plus.PEHeader
{
    internal class PEResult : IPEResult
    {
        public byte[] RawData { get; }

        public PEResult(byte[] rawData) => RawData = rawData;
    }
}