namespace Llama.PE.Structures.Sections.Idata
{
    using System;
    using System.Runtime.InteropServices;

    [StructLayout(LayoutKind.Sequential, Pack = 1)]
    public struct ImportLookupEntryPE32Plus : IEquatable<ImportLookupEntryPE32Plus>
    {
        public ulong Entry;

        private const ulong OrdinalFlagBit = 0x8000000000000000;

        public bool OrdinalFlag
        {
            get => (Entry & OrdinalFlagBit) != 0;
            set
            {
                if (value)
                    Entry |= OrdinalFlagBit;
                else
                    Entry &= ~OrdinalFlagBit;
            }
        }

        public ushort OrdinalNumber
        {
            get => (ushort)Entry;
            set => Entry = ((Entry >> 16) << 16) | value;
        }

        public uint HintNameTableRVA
        {
            get => (uint)(Entry & int.MaxValue);
            set => Entry = ((Entry >> 31) << 31) | value;
        }

        public bool Equals(ImportLookupEntryPE32Plus other) => Entry == other.Entry;

        public override bool Equals(object obj) => obj is ImportLookupEntryPE32Plus other && Equals(other);

        public override int GetHashCode() => Entry.GetHashCode();

        public static bool operator ==(ImportLookupEntryPE32Plus left, ImportLookupEntryPE32Plus right) => left.Equals(right);

        public static bool operator !=(ImportLookupEntryPE32Plus left, ImportLookupEntryPE32Plus right) => !left.Equals(right);
    }
}