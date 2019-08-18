namespace Llama.PE.Idata
{
    using System.Runtime.InteropServices;

    [StructLayout(LayoutKind.Sequential, Pack = 1)]
    public struct ImportLookupEntryPE32Plus
    {
        public ulong Entry;

        public byte OrdinalFlag => (Entry & 0x8000000000000000) == 0 ? (byte)0 : (byte)1; // bit 63
        public ushort OrdinalNumber => (ushort)Entry; // bits 0-15 if OrdinalFlag == 1
        public uint HintNameTableRVA => (uint)(Entry & int.MaxValue); // bits 0-30 if OrdinalFlag == 0
    }
}