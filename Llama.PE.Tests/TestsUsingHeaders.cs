namespace Llama.PE.Tests
{
    using System.Text;
    using BinaryUtils;
    using Converters;
    using Structures.Header;

    public abstract class TestsUsingHeaders
    {
        protected SectionHeader[] SectionHeaders => Image.SectionHeaders;
        protected PE32PlusOptionalHeader OptionalHeader => Image.OptionalHeader;
        protected MZHeader MZHeader => Image.MZHeader;
        protected PEHeader PEHeader => Image.PEHeader;
        private readonly IStructReader _reader;
        protected readonly IPE32PlusContext Image;

        protected readonly byte[] TestFile;

        protected TestsUsingHeaders(byte[] testFile)
        {
            TestFile = testFile;
            _reader = new ArrayStructReaderWriter(TestFile);
            Image = new PE32PlusReaderContext(_reader);
        }

        protected string ReadStringFromRVA(ulong rva)
        {
            var offset = Image.GetFileOffset(rva);
            _reader.Offset = offset;
            var importName = _reader.ReadUntilNull<byte>();
            return Encoding.ASCII.GetString(importName.ToArray());
        }
    }
}