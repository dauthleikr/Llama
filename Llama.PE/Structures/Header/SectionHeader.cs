namespace Llama.PE.Structures.Header
{
    using System;
    using System.Reflection.PortableExecutable;
    using System.Runtime.InteropServices;
    using System.Text;

    [StructLayout(LayoutKind.Sequential, CharSet = CharSet.Ansi, Pack = 1)]
    public struct SectionHeader : IEquatable<SectionHeader>
    {
        public const int NameSize = 8;

        public ulong Name;
        public uint VirtualSize;

        /// <summary>
        ///     RVA when loaded into memory by the OS
        /// </summary>
        public uint VirtualAddress;

        /// <summary>
        ///     If this is less than VirtualSize, the remaining bytes are initialized with zeroes
        /// </summary>
        public uint SizeOfRawData;

        /// <summary>
        ///     RVA on file
        /// </summary>
        public uint PointerToRawData;

        public uint PointerToRelocations;
        public uint PointerToLinenumbers;
        public ushort NumberOfRelocations;
        public ushort NumberOfLinenumbers;
        public SectionCharacteristics Characteristics;

        public string NameString => Encoding.ASCII.GetString(BitConverter.GetBytes(Name));

        public bool Equals(SectionHeader other) =>
            Name == other.Name &&
            VirtualSize == other.VirtualSize &&
            VirtualAddress == other.VirtualAddress &&
            SizeOfRawData == other.SizeOfRawData &&
            PointerToRawData == other.PointerToRawData &&
            PointerToRelocations == other.PointerToRelocations &&
            PointerToLinenumbers == other.PointerToLinenumbers &&
            NumberOfRelocations == other.NumberOfRelocations &&
            NumberOfLinenumbers == other.NumberOfLinenumbers &&
            Characteristics == other.Characteristics;

        public override bool Equals(object obj) => obj is SectionHeader other && Equals(other);

        public override int GetHashCode()
        {
            unchecked
            {
                var hashCode = Name.GetHashCode();
                hashCode = (hashCode * 397) ^ (int)VirtualSize;
                hashCode = (hashCode * 397) ^ (int)VirtualAddress;
                hashCode = (hashCode * 397) ^ (int)SizeOfRawData;
                hashCode = (hashCode * 397) ^ (int)PointerToRawData;
                hashCode = (hashCode * 397) ^ (int)PointerToRelocations;
                hashCode = (hashCode * 397) ^ (int)PointerToLinenumbers;
                hashCode = (hashCode * 397) ^ NumberOfRelocations.GetHashCode();
                hashCode = (hashCode * 397) ^ NumberOfLinenumbers.GetHashCode();
                hashCode = (hashCode * 397) ^ (int)Characteristics;
                return hashCode;
            }
        }

        public static bool operator ==(SectionHeader left, SectionHeader right) => left.Equals(right);

        public static bool operator !=(SectionHeader left, SectionHeader right) => !left.Equals(right);
    }
}