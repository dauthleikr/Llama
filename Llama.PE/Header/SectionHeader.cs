namespace Llama.PE.Header
{
    using System.Reflection.PortableExecutable;
    using System.Runtime.InteropServices;
    using System.Text;

    [StructLayout(LayoutKind.Sequential, CharSet = CharSet.Ansi, Pack = 1)]
    public unsafe struct SectionHeader
    {
        public const int NameSize = 8;

        public fixed byte Name[NameSize];
        public uint VirtualSize;
        public uint VirtualAddress;

        /// <summary>
        ///     If this is less than VirtualSize, the remaining bytes are initialized with zeroes
        /// </summary>
        public uint SizeOfRawData;

        public uint PointerToRawData;
        public uint PointerToRelocations;
        public uint PointerToLinenumbers;
        public ushort NumberOfRelocations;
        public ushort NumberOfLinenumbers;
        public SectionCharacteristics Characteristics;

        public string NameString
        {
            get
            {
                fixed (byte* ptr = Name)
                    return Encoding.ASCII.GetString(ptr, 8);
            }
        }
    }
}