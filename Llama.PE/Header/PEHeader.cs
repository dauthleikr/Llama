namespace Llama.PE.Header
{
    using System.Reflection.PortableExecutable;
    using System.Runtime.InteropServices;

    [StructLayout(LayoutKind.Sequential, CharSet = CharSet.Ansi, Pack = 1)]
    public unsafe struct PEHeader
    {
        public fixed byte Magic[4];
        public Machine Architecture;
        public ushort NumberOfSections;
        public uint CreationTimePOSIX;
        public uint SymbolTableRVA;
        public uint NumberOfSymbols;
        public ushort OptionalHeaderSize;
        public Characteristics Characteristics;
    }
}