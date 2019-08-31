namespace Llama.PE.Structures.Sections.Reloc
{
    using System;
    using System.Runtime.InteropServices;

    [StructLayout(LayoutKind.Sequential, Pack = 1)]
    public struct BaseRelocationBlockEntry : IEquatable<BaseRelocationBlockEntry>
    {
        public ushort Entry;

        public BaseRelocationType Type
        {
            get => (BaseRelocationType)((Entry & 0xF000) >> 12);
            set => Entry = (ushort)((Entry & 0x0FFF) | (((int)value << 12) & 0xF000));
        }

        public int Offset
        {
            get => Entry & 0x0FFF;
            set => Entry = (ushort)((Entry & 0xF000) | (value & 0x0FFF));
        }

        public bool Equals(BaseRelocationBlockEntry other) => Entry == other.Entry;

        public override bool Equals(object obj) => obj is BaseRelocationBlockEntry other && Equals(other);

        public override int GetHashCode() => Entry.GetHashCode();

        public static bool operator ==(BaseRelocationBlockEntry left, BaseRelocationBlockEntry right) => left.Equals(right);

        public static bool operator !=(BaseRelocationBlockEntry left, BaseRelocationBlockEntry right) => !left.Equals(right);
    }
}