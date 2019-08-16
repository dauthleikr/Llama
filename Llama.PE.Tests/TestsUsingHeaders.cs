namespace Llama.PE.Tests
{
    using System;
    using System.Runtime.InteropServices;

    public abstract class TestsUsingHeaders
    {
        protected readonly MZHeader MZHeader;
        protected readonly PE32PlusOptionalHeader OptionalHeader;
        protected readonly PEHeader PEHeader;
        protected readonly SectionHeader[] SectionHeaders;
        protected readonly byte[] TestFile;

        public unsafe TestsUsingHeaders(byte[] testFile)
        {
            TestFile = testFile;
            if (sizeof(MZHeader) > testFile.Length)
                throw new Exception("Bad test exe file");

            MZHeader = GetStruct<MZHeader>(0);
            if (sizeof(PEHeader) + sizeof(PE32PlusOptionalHeader) + MZHeader.NewHeaderRVA > testFile.Length)
                throw new Exception("Bad test exe file (cannot fit PE optional header)");

            PEHeader = GetStruct<PEHeader>(MZHeader.NewHeaderRVA);
            OptionalHeader = GetStruct<PE32PlusOptionalHeader>((uint)(MZHeader.NewHeaderRVA + sizeof(PEHeader)));
            if (PEHeader.OptionalHeaderSize + MZHeader.NewHeaderRVA + sizeof(PEHeader) + sizeof(SectionHeader) * PEHeader.NumberOfSections > testFile.Length)
                throw new Exception("Bad test exe file (cannot fit section headers)");

            SectionHeaders = new SectionHeader[PEHeader.NumberOfSections];
            for (var i = 0; i < SectionHeaders.Length; i++)
                SectionHeaders[i] = GetStruct<SectionHeader>((uint)(MZHeader.NewHeaderRVA + PEHeader.OptionalHeaderSize + sizeof(PEHeader) + i * sizeof(SectionHeader)));
        }

        private T GetStruct<T>(uint rva) where T : struct => MemoryMarshal.Read<T>(new ReadOnlySpan<byte>(TestFile, (int)rva, TestFile.Length - (int)rva));
    }
}