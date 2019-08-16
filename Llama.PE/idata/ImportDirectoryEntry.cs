namespace Llama.PE.idata
{
    using System.Runtime.InteropServices;

    [StructLayout(LayoutKind.Sequential, CharSet = CharSet.Ansi, Pack = 1)]
    public struct ImportDirectoryEntry
    {
        public uint ImportLookupTableRVA;
        public uint CreationTimePOSIX;
        public uint ForwarderChain;
        public uint NameRVA;
        public uint IAT_RVA;
    }
}