namespace Llama.PE.Idata
{
    using System;
    using System.Runtime.InteropServices;
    using Microsoft.VisualBasic.CompilerServices;

    [StructLayout(LayoutKind.Sequential, Pack = 1)]
    public struct ImportDirectoryEntry : IEquatable<ImportDirectoryEntry>
    {
        public uint ImportLookupTableRVA;
        public uint CreationTimePOSIX;
        public uint ForwarderChain;
        public uint NameRVA;
        public uint IAT_RVA;

        public bool Equals(ImportDirectoryEntry other) =>
            ImportLookupTableRVA == other.ImportLookupTableRVA &&
            CreationTimePOSIX == other.CreationTimePOSIX &&
            ForwarderChain == other.ForwarderChain &&
            NameRVA == other.NameRVA &&
            IAT_RVA == other.IAT_RVA;

        public static bool operator ==(ImportDirectoryEntry left, ImportDirectoryEntry right) => left.Equals(right);
        public static bool operator !=(ImportDirectoryEntry left, ImportDirectoryEntry right) => !(left == right);

        public override bool Equals(object obj) => obj is ImportDirectoryEntry other && Equals(other);

        public override int GetHashCode()
        {
            unchecked
            {
                var hashCode = (int)ImportLookupTableRVA;
                hashCode = (hashCode * 397) ^ (int)CreationTimePOSIX;
                hashCode = (hashCode * 397) ^ (int)ForwarderChain;
                hashCode = (hashCode * 397) ^ (int)NameRVA;
                hashCode = (hashCode * 397) ^ (int)IAT_RVA;
                return hashCode;
            }
        }
    }
}