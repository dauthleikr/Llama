namespace Llama.PE.Idata
{
    using System.Runtime.InteropServices;

    [StructLayout(LayoutKind.Sequential, Pack = 1)]
    public struct ImportDirectoryEntry
    {
        public uint ImportLookupTableRVA;
        public uint CreationTimePOSIX;
        public uint ForwarderChain;
        public uint NameRVA;
        public uint IAT_RVA;
    }
}