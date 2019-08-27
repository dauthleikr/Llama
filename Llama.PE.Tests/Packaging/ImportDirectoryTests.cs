namespace Llama.PE.Tests.Packaging
{
    using System.Linq;
    using System.Text;
    using BinaryUtils;
    using Converters;
    using Moq;
    using NUnit.Framework;
    using PE.Packaging.PE32Plus.Idata;

    [TestFixture]
    internal class ImportDirectoryTests
    {
        [SetUp]
        public void SetUp()
        {
            var idataInfo = new Mock<IIdataInfo>();
            idataInfo.Setup(item => item.IdataRVA).Returns(TestIdataRVA);
            idataInfo.Setup(item => item.IATBlockSize).Returns(32);
            idataInfo.Setup(item => item.Imports)
                .Returns(
                    new (string library, string function)[]
                    {
                        ("kernel32.dll", "WriteFile"),
                        ("kernel32.dll", "VirtualAllocEx"),
                        ("user32.dll", "MessageBox")
                    }
                );

            var packager = new IdataSectionPackager(new HintNameEntryWriter());
            _packaged = packager.Package(idataInfo.Object);
        }

        private const int TestIdataRVA = 10000;
        private IIdataResult _packaged;

        [Test]
        public void CanResolveFunctionNames()
        {
            var rva = _packaged.IATResolver.GetRVAOfIATEntry("kernel32.dll", "VirtualAllocEx");
            Assert.GreaterOrEqual(rva, _packaged.IdataRVA, "IAT entry RVA should be in idata");
            var reader = new ArrayStructReaderWriter(_packaged.RawData)
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
            var reader = new ArrayStructReaderWriter(_packaged.RawData.ToArray())
            {
                Offset = _packaged.ImportDirectory.VirtualAddress - _packaged.IdataRVA
            };
            reader.Offset += 12;
            var nameRVA = reader.Read<uint>();
            Assert.NotZero(nameRVA, "NameRVA should never be 0");
            reader.Offset = nameRVA - _packaged.IdataRVA;
            var name = Encoding.ASCII.GetString(reader.ReadUntilNull<byte>().ToArray());
            Assert.AreEqual("kernel32.dll", name);

            reader.Offset = _packaged.ImportDirectory.VirtualAddress - _packaged.IdataRVA + 32;
            nameRVA = reader.Read<uint>();
            Assert.NotZero(nameRVA, "NameRVA should never be 0");
            reader.Offset = nameRVA - _packaged.IdataRVA;
            name = Encoding.ASCII.GetString(reader.ReadUntilNull<byte>().ToArray());
            Assert.AreEqual("user32.dll", name);
        }

        [Test]
        public void IAT_RVAMakesSense()
        {
            Assert.GreaterOrEqual(_packaged.IAT.VirtualAddress, _packaged.IdataRVA, "IAT start should be in idata");
        }

        [Test]
        public void IdataRVAIsCorrect()
        {
            Assert.AreEqual(_packaged.IdataRVA, TestIdataRVA);
        }

        [Test]
        public void ImportDirectoryTableRVAMakesSense()
        {
            Assert.GreaterOrEqual(_packaged.ImportDirectory.VirtualAddress, _packaged.IdataRVA, "Import directory table start should be in idata");
        }

        [Test]
        public void NumberOfImportDirectoryEntriesIsCorrect()
        {
            var reader = new ArrayStructReaderWriter(_packaged.RawData.ToArray())
            {
                Offset = _packaged.ImportDirectory.VirtualAddress - _packaged.IdataRVA
            };
            Assert.NotZero(reader.Read<uint>(), "ImportLookupTableRVA should not be 0");
            reader.Offset += 16; // ImportLookupTableRVA of next entry
            Assert.NotZero(reader.Read<uint>(), "ImportLookupTableRVA should not be 0");
            reader.Offset += 16; // ImportLookupTableRVA of next entry
            Assert.Zero(reader.Read<uint>(), "ImportLookupTableRVA should be 0 (2 entries expected)");
        }
    }
}