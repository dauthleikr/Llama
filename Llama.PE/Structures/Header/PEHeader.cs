namespace Llama.PE.Structures.Header
{
    using System;
    using System.Reflection.PortableExecutable;
    using System.Runtime.InteropServices;
    using System.Text;

    [StructLayout(LayoutKind.Sequential, CharSet = CharSet.Ansi, Pack = 1)]
    public struct PEHeader
    {
        public uint Magic;
        public Machine Architecture;
        public ushort NumberOfSections;
        public uint CreationTimePOSIX;
        public uint SymbolTableRVA;
        public uint NumberOfSymbols;
        public ushort OptionalHeaderSize;
        public Characteristics Characteristics;

        public string MagicString => Encoding.ASCII.GetString(BitConverter.GetBytes(Magic));
    }
}