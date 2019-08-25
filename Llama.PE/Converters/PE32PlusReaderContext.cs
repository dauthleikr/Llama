namespace Llama.PE.Converters
{
    using BinaryUtils;
    using Structures.Header;

    public class PE32PlusReaderContext : IPE32PlusContext
    {
        public MZHeader MZHeader { get; }
        public PEHeader PEHeader { get; }
        public PE32PlusOptionalHeader OptionalHeader { get; }
        public SectionHeader[] SectionHeaders { get; }

        public unsafe PE32PlusReaderContext(IStructReader peReader)
        {
            MZHeader = peReader.Read<MZHeader>();
            PEHeader = peReader.Read<PEHeader>(MZHeader.NewHeaderRVA);
            OptionalHeader = peReader.Read<PE32PlusOptionalHeader>((uint)(MZHeader.NewHeaderRVA + sizeof(PEHeader)));
            SectionHeaders = new SectionHeader[PEHeader.NumberOfSections];
            for (var i = 0; i < SectionHeaders.Length; i++)
                SectionHeaders[i] = peReader.Read<SectionHeader>((uint)(MZHeader.NewHeaderRVA + PEHeader.OptionalHeaderSize + sizeof(PEHeader) + i * sizeof(SectionHeader)));
        }

        public ulong GetFileOffset(ulong rva)
        {
            foreach (var section in SectionHeaders)
                if (section.VirtualAddress <= rva && section.VirtualAddress + section.VirtualSize > rva)
                    return section.PointerToRawData + rva - section.VirtualAddress;

            return rva;
        }
    }
}