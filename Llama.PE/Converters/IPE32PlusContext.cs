namespace Llama.PE.Converters
{
    using Structures.Header;

    public interface IPE32PlusContext
    {
        MZHeader MZHeader { get; }
        PEHeader PEHeader { get; }
        PE32PlusOptionalHeader OptionalHeader { get; }
        SectionHeader[] SectionHeaders { get; }

        ulong GetFileOffset(ulong rva);
    }
}