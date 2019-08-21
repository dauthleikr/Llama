namespace Llama.PE.Tests
{
    using System;
    using System.IO;
    using System.Linq;
    using BinaryUtils;
    using Header;
    using Idata;
    using NUnit.Framework;
    using NUnit.Framework.Constraints;

    [TestFixture]
    internal unsafe class IdataTests : TestsUsingHeaders
    {
        private ImportDirectory _importDirectory;

        public IdataTests() : base(File.ReadAllBytes("test.exe")) { }

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