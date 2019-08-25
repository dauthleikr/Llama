namespace Llama.PE.Tests.Packaging
{
    using System.Collections.Generic;
    using System.Text;
    using BinaryUtils;
    using Converters;
    using NUnit.Framework;
    using PE.Packaging.PE32Plus.Idata;

    [TestFixture]
    internal class IdataTests
    {
        [SetUp]
        public void SetUp()
        {
            var packager = new IdataSectionPackager(new HintNameEntryWriter());
            _packaged = packager.Package(new IdataInfo());
        }

        private const int TestIdataRVA = 10000;
        private IIdataResult _packaged;

        private class IdataInfo : IIdataInfo
        {
            public IEnumerable<(string library, string function)> Imports =>
                new (string library, string function)[]
                {
                    ("kernel32.dll", "WriteFile"),
                    ("kernel32.dll", "VirtualAllocEx"),
                    ("user32.dll", "MessageBox")
                };

            public uint IdataRVA => TestIdataRVA;
            public uint IATBlockSize => 32;
        }

        [Test]
        public void CanResolveFunctionNames()
        {
            var rva = _packaged.GetRVAOfIATEntry("kernel32.dll", "VirtualAllocEx");
            Assert.GreaterOrEqual(rva, _packaged.IdataRVA, "IAT entry RVA should be in idata");
            var reader = new ArrayStructReaderWriter(_packaged.RawData.ToArray())
            {
                Offset = rva - _packaged.IdataRVA
            };
            var hintNameRVA = reader.Read<uint>();
            Assert.NotZero(hintNameRVA);
            reader.Offset = hintNameRVA - _packaged.IdataRVA + 2; // name is on offset 2 of Hint/Name struct
            var name = Encoding.ASCII.GetString(reader.ReadUntilNull<byte>().ToArray());
            Assert.AreEqual("VirtualAllocEx", name);
        }

        [Test]
        public void CanResolveLibraryNames()
        {
            var reader = new ArrayStructReaderWriter(_packaged.RawData.ToArray()) {Offset = _packaged.ImportDirectoryTableRVA - _packaged.IdataRVA};
            reader.Offset += 12;
            var nameRVA = reader.Read<uint>();
            Assert.NotZero(nameRVA, "NameRVA should never be 0");
            reader.Offset = nameRVA - _packaged.IdataRVA;
            var name = Encoding.ASCII.GetString(reader.ReadUntilNull<byte>().ToArray());
            Assert.AreEqual("kernel32.dll", name);

            reader.Offset = _packaged.ImportDirectoryTableRVA - _packaged.IdataRVA + 32;
            nameRVA = reader.Read<uint>();
            Assert.NotZero(nameRVA, "NameRVA should never be 0");
            reader.Offset = nameRVA - _packaged.IdataRVA;
            name = Encoding.ASCII.GetString(reader.ReadUntilNull<byte>().ToArray());
            Assert.AreEqual("user32.dll", name);
        }

        [Test]
        public void IAT_RVAMakesSense()
        {
            Assert.GreaterOrEqual(_packaged.IAT_RVA, _packaged.IdataRVA, "IAT start should be in idata");
        }

        [Test]
        public void IdataRVAIsCorrect()
        {
            Assert.AreEqual(_packaged.IdataRVA, TestIdataRVA);
        }

        [Test]
        public void ImportDirectoryTableRVAMakesSense()
        {
            Assert.GreaterOrEqual(_packaged.ImportDirectoryTableRVA, _packaged.IdataRVA, "Import directory table start should be in idata");
        }

        [Test]
        public void NumberOfImportDirectoryEntriesIsCorrect()
        {
            var reader = new ArrayStructReaderWriter(_packaged.RawData.ToArray()) {Offset = _packaged.ImportDirectoryTableRVA - _packaged.IdataRVA};
            Assert.NotZero(reader.Read<uint>(), "ImportLookupTableRVA should not be 0");
            reader.Offset += 16; // ImportLookupTableRVA of next entry
            Assert.NotZero(reader.Read<uint>(), "ImportLookupTableRVA should not be 0");
            reader.Offset += 16; // ImportLookupTableRVA of next entry
            Assert.Zero(reader.Read<uint>(), "ImportLookupTableRVA should be 0 (2 entries expected)");
        }
    }
}