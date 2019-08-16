namespace Llama.BinaryUtils.Tests
{
    using System.Runtime.InteropServices;

    [StructLayout(LayoutKind.Sequential, CharSet = CharSet.Ansi, Pack = 2)]
    internal struct TestStruct2
    {
        public ulong Value2;
        public byte Value;
    }
}