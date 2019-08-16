namespace Llama.BinaryUtils.Tests
{
    using System.Runtime.InteropServices;

    [StructLayout(LayoutKind.Sequential, Pack = 1)]
    internal unsafe struct TestStruct1
    {
        public int Value;
        public fixed byte Value2[2];
    }
}