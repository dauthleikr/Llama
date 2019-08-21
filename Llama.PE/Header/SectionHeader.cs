namespace Llama.PE.Header
{
    using System;
    using System.Reflection.PortableExecutable;
    using System.Runtime.InteropServices;
    using System.Text;

    [StructLayout(LayoutKind.Sequential, CharSet = CharSet.Ansi, Pack = 1)]
    public struct SectionHeader
    {
        public const int NameSize = 8;

        public ulong Name;
        public uint VirtualSize;

        /// <summary>
        /// RVA when loaded into memory by the OS
        /// </summary>
        public uint VirtualAddress;

        /// <summary>
        ///     If this is less than VirtualSize, the remaining bytes are initialized with zeroes
        /// </summary>
        public uint SizeOfRawData;

        /// <summary>
        /// RVA on file
        /// </summary>
        public uint PointerToRawData;
        public uint PointerToRelocations;
        public uint PointerToLinenumbers;
        public ushort NumberOfRelocations;
        public ushort NumberOfLinenumbers;
        public SectionCharacteristics Characteristics;

        public string NameString => Encoding.ASCII.GetString(BitConverter.GetBytes(Name));
    }
}