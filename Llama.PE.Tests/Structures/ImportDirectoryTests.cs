namespace Llama.PE.Tests.Structures
{
    using System.IO;
    using System.Linq;
    using BinaryUtils;
    using Converters;
    using NUnit.Framework;
    using PE.Structures.Sections.Idata;

    [TestFixture]
    internal unsafe class IdataTests : TestsUsingHeaders
    {
        private ImportDirectory _importDirectory;

        public IdataTests() : base(File.ReadAllBytes(@"ValidPEs\ninow.exe")) { }

        [OneTimeSetUp]
        public void Init()
        {
            var importDirectoryMapper = new ImportDirectoryReader(new HintNameEntryReader());
            var structReader = new ArrayStructReaderWriter(TestFile)
            {
                Offset = Image.GetFileOffset(OptionalHeader.DataDirectories.ImportTable.VirtualAddress)
            };
            _importDirectory = importDirectoryMapper.Read(structReader, Image);
        }

        [Test]
        public void CanResolveHintNames()
        {
            var functionNames = _importDirectory.HintOrNameTable.Select(item => item.Name).ToArray();

            Assert.That(functionNames.All(func => !string.IsNullOrWhiteSpace(func)));
            Assert.That(functionNames.Contains("HeapAlloc") || functionNames.Contains("WriteFile"));
        }

        [Test]
        public void CanResolveImportLibraryNames()
        {
            var importLibraryNames = _importDirectory.ImportDirectoryTable.Select(item => ReadStringFromRVA(item.NameRVA).ToUpper()).ToArray();

            Assert.That(importLibraryNames.All(lib => !string.IsNullOrWhiteSpace(lib)));
            Assert.Contains("KERNEL32.DLL", importLibraryNames);
        }

        [Test]
        public void ImportDirectoryEntryHasTheRightSize()
        {
            Assert.AreEqual(20, sizeof(ImportDirectoryEntry));
        }
    }
}