namespace Llama.PE
{
    using System;
    using System.Runtime.InteropServices;

    [StructLayout(LayoutKind.Sequential, CharSet = CharSet.Ansi, Pack = 1)]
    public unsafe struct MZHeader
    {
        public fixed byte Magic[2]; // MZ
        public ushort NumberOfBytesInLastPage;
        public ushort NumberOfPages;
        public ushort NumerOfRelocationEntries;
        public ushort NumerOfHeaderParagraphs;
        public ushort MinimumAllocatedParagraphs;
        public ushort MaximumAllocatedParagraphs;
        public ushort InitialStackSegment;
        public ushort InitialStackPointer;
        public ushort Checksum;
        public uint EntryPointRVA;
        public ushort RelocationTableRVA;
        public ushort OverlayNumber;
        public fixed ushort Reserved1[4];
        public ushort OemId;
        public ushort OemInfo;
        public fixed ushort Reserved2[10];
        public uint NewHeaderRVA; // e_lfanew
        public fixed byte MSDOSStub[MSDOSStubLength];

        private const int MSDOSStubLength = 64;

        public byte[] CreateMSDOSStubArray()
        {
            fixed (byte* ptr = MSDOSStub)
                return new Span<byte>(ptr, MSDOSStubLength).ToArray();
        }

        public static MZHeader CreateWithDefaultStub(uint newHeaderRVA, ushort initialStackPointer, ushort numberOfHeaderParagraphs, ushort numberOfRelocationEntries)
        {
            var msdosStub = new byte[]
            {
                14, 31, 186, 14, 0, 180, 9, 205, 33, 184, 1, 76, 205, 33, 84, 104, 105, 115, 32, 112, 114, 111, 103, 114, 97, 109, 32, 99, 97, 110, 110, 111, 116, 32, 98, 101, 32, 114, 117, 110, 32,
                105, 110, 32, 68, 79, 83, 32, 109, 111, 100, 101, 46, 13, 13, 10, 36, 0, 0, 0, 0, 0, 0, 0
            };

            // ReSharper disable once UseObjectOrCollectionInitializer
            var header = new MZHeader
            {
                Checksum = 0,
                EntryPointRVA = 0,
                InitialStackPointer = initialStackPointer,
                InitialStackSegment = 0,
                MaximumAllocatedParagraphs = ushort.MaxValue,
                MinimumAllocatedParagraphs = 0,
                NewHeaderRVA = newHeaderRVA,
                NumberOfBytesInLastPage = 144,
                NumberOfPages = 3,
                NumerOfHeaderParagraphs = numberOfHeaderParagraphs,
                NumerOfRelocationEntries = numberOfRelocationEntries,
                OemId = 0,
                OemInfo = 0
            };

            header.Magic[0] = (byte)'M';
            header.Magic[1] = (byte)'Z';
            for (var i = 0; i < msdosStub.Length; i++)
                header.MSDOSStub[i] = msdosStub[i];

            return header;
        }
    }
}