namespace Llama.PE.Structures.Header
{
    using System.Runtime.InteropServices;

    [StructLayout(LayoutKind.Sequential, CharSet = CharSet.Ansi, Pack = 1)]
    public struct ImageDataDirectory
    {
        public uint VirtualAddress;
        public uint Size;
    }
}