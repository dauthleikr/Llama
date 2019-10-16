namespace Llama.Linker
{
    internal class ConstDataFixup : IFixupInfo
    {
        public long Position { get; set; }
        public byte[] ConstData { get; }

        public ConstDataFixup(long position, byte[] constData)
        {
            Position = position;
            ConstData = constData;
        }
    }
}