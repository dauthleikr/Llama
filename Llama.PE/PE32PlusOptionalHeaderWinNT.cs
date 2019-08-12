namespace Llama.PE
{
    using System.Reflection.PortableExecutable;
    using System.Runtime.InteropServices;

    /// <summary>
    /// The Windows-specific part of the <see cref="PE32PlusOptionalHeader"/> header.
    /// </summary>
    [StructLayout(LayoutKind.Sequential, CharSet = CharSet.Ansi, Pack = 1)]
    public struct PE32PlusOptionalHeaderWinNT
    {
        public ulong ImageBaseOffset;
        public uint SectionAlignment;
        public uint FileAlignment;
        public ushort MajorOSVersion;
        public ushort MinorOSVersion;
        public ushort MajorImageVersion;
        public ushort MinorImageVersion;
        public ushort MajorSubsystemVersion;
        public ushort MinorSubsystemVersion;
        /// <summary>
        /// Reserved, must be 0
        /// </summary>
        public uint Win32VersionValue;
        /// <summary>
        /// Full image size, must be a multiple of <see cref="SectionAlignment"/>
        /// </summary>
        public uint SizeOfImage;
        /// <summary>
        /// Combined size of all headers, must be a multiple of <see cref="FileAlignment"/>
        /// </summary>
        public uint SizeOfHeaders;

        public uint Checksum;
        public Subsystem Subsystem;
        public DllCharacteristics DllCharacteristics;
        public ulong StackSizeReserve;
        public ulong StackSizeCommit;
        public ulong HeapSizeReserve;
        public ulong HeapSizeCommit;
        /// <summary>
        /// Reserved, must be 0
        /// </summary>
        public uint LoaderFlags;
        public uint NumberOfDataDirectoryEntries;
    }
}