namespace Llama.PE.Header
{
    using System.Runtime.InteropServices;

    /// <summary>
    ///     The standard part of the <see cref="PE32PlusOptionalHeader" /> header.
    /// </summary>
    [StructLayout(LayoutKind.Sequential, CharSet = CharSet.Ansi, Pack = 1)]
    public struct PE32PlusOptionalHeaderStandard
    {
        public ExecutableKind ExecutableKind;
        public byte MajorLinkerVersion;
        public byte MinorLinkerVersion;
        public uint SizeOfCode;
        public uint SizeOfInitializedData;
        public uint SizeOfUninitializedData;
        public uint EntryPointRVA;
        public uint BaseOfCodeRVA;
        // The normal PE32 header would also have "uint BaseOfData" here
    }
}