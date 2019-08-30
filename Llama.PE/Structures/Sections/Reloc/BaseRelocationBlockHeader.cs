namespace Llama.PE.Structures.Sections.Reloc
{
    using System;
    using System.Runtime.InteropServices;

    [StructLayout(LayoutKind.Sequential, Pack = 1)]
    public struct BaseRelocationBlockHeader : IEquatable<BaseRelocationBlockHeader>
    {
        public uint PageRVA;
        public uint BlockSize;

        public bool Equals(BaseRelocationBlockHeader other) => PageRVA == other.PageRVA && BlockSize == other.BlockSize;

        public override bool Equals(object obj) => obj is BaseRelocationBlockHeader other && Equals(other);

        public override int GetHashCode()
        {
            unchecked
            {
                return ((int)PageRVA * 397) ^ (int)BlockSize;
            }
        }

        public static bool operator ==(BaseRelocationBlockHeader left, BaseRelocationBlockHeader right) => left.Equals(right);

        public static bool operator !=(BaseRelocationBlockHeader left, BaseRelocationBlockHeader right) => !left.Equals(right);
    }
}