namespace Llama.PE.Structures.Header
{
    using System;
    using System.Runtime.InteropServices;

    [StructLayout(LayoutKind.Sequential, CharSet = CharSet.Ansi, Pack = 1)]
    public struct ImageDataDirectory : IEquatable<ImageDataDirectory>
    {
        public uint VirtualAddress;
        public uint Size;

        public bool Equals(ImageDataDirectory other) => VirtualAddress == other.VirtualAddress && Size == other.Size;

        public override bool Equals(object obj) => obj is ImageDataDirectory other && Equals(other);

        public override int GetHashCode()
        {
            unchecked
            {
                return ((int)VirtualAddress * 397) ^ (int)Size;
            }
        }

        public static bool operator ==(ImageDataDirectory left, ImageDataDirectory right) => left.Equals(right);

        public static bool operator !=(ImageDataDirectory left, ImageDataDirectory right) => !left.Equals(right);
    }
}