namespace Llama.PE.Tests.Packaging
{
    using System;
    using System.Linq;
    using System.Reflection.PortableExecutable;
    using System.Runtime.InteropServices;
    using System.Text;
    using BinaryUtils;
    using Moq;
    using NUnit.Framework;
    using PE.Packaging.PE32Plus.MZHeader;
    using PE.Packaging.PE32Plus.OptionalHeader;
    using PE.Packaging.PE32Plus.Sections;
    using PE.Structures.Header;
    using SectionHeader = PE.Structures.Header.SectionHeader;

    [TestFixture]
    internal class PEOptionalHeaderTests
    {
        [OneTimeSetUp]
        public void SetUp()
        {
            var mzResult = new Mock<IMZResult>();
            mzResult.Setup(item => item.RawData).Returns(new byte[Marshal.SizeOf<MZHeader>()]);
            mzResult.Setup(item => item.NewHeaderOffset).Returns((uint)Marshal.SizeOf<MZHeader>());

            var sectionsResult = new Mock<ISectionsResult>();
            sectionsResult.Setup(item => item.SectionHeaders).Returns(Sections);
            sectionsResult.Setup(item => item.SectionHeaders).Returns(Sections);
            sectionsResult.Setup(item => item.EntryPointRVA).Returns(EntryPointRVA);
            sectionsResult.Setup(item => item.IAT).Returns(DataDirectoryIAT);
            sectionsResult.Setup(item => item.ImportTable).Returns(DataDirectoryImportTable);
            sectionsResult.Setup(item => item.RawData).Returns(new byte[RawDataSize]);

            var optHeaderInfo = new Mock<IOptionalHeaderInfo>();
            optHeaderInfo.Setup(item => item.MZHeader).Returns(mzResult.Object);
            optHeaderInfo.Setup(item => item.Sections).Returns(sectionsResult.Object);
            optHeaderInfo.Setup(item => item.Subsystem).Returns(Subsystem.WindowsCui);
            optHeaderInfo.Setup(item => item.DllCharacteristics)
                .Returns(
                    DllCharacteristics.HighEntropyVirtualAddressSpace |
                    DllCharacteristics.DynamicBase |
                    DllCharacteristics.NxCompatible |
                    DllCharacteristics.TerminalServerAware
                );
            optHeaderInfo.Setup(item => item.ImageBase).Returns(0x400000);
            optHeaderInfo.Setup(item => item.FileAlignment).Returns(0x200);
            optHeaderInfo.Setup(item => item.SectionAlignment).Returns(0x1000);

            var packager = new OptionalHeaderPackager();
            _packaged = packager.Package(optHeaderInfo.Object);
        }

        private IOptionalHeaderResult _packaged;

        private static readonly SectionHeader[] Sections =
        {
            new SectionHeader
            {
                Name = BitConverter.ToUInt64(Encoding.ASCII.GetBytes(".text\0\0\0")),
                VirtualAddress = 0x1000,
                VirtualSize = 0x1000
            },
            new SectionHeader
            {
                Name = BitConverter.ToUInt64(Encoding.ASCII.GetBytes(".idata\0\0")),
                VirtualAddress = 0x2000,
                VirtualSize = 0x1000
            },
            new SectionHeader
            {
                Name = BitConverter.ToUInt64(Encoding.ASCII.GetBytes(".data\0\0\0")),
                VirtualAddress = 0x3000,
                VirtualSize = 0x1000
            }
        };

        private static readonly ImageDataDirectory DataDirectoryIAT = new ImageDataDirectory
        {
            Size = 0x100,
            VirtualAddress = 0x2000
        };

        private static readonly ImageDataDirectory DataDirectoryImportTable = new ImageDataDirectory
        {
            Size = 0x100,
            VirtualAddress = 0x2500
        };

        private const uint EntryPointRVA = 0x1123;
        private const uint RawDataSize = 0x2000;

        [Test]
        public void CanReadProperties()
        {
            var reader = new ArrayStructReaderWriter(_packaged.RawData);
            var optHeader = reader.Read<PE32PlusOptionalHeader>();

            Assert.AreEqual(EntryPointRVA, optHeader.Standard.EntryPointRVA);
            Assert.AreEqual(Sections.First(sec => sec.NameString.StartsWith(".text")).VirtualSize, optHeader.Standard.SizeOfCode);
            Assert.AreEqual(Sections.First(sec => sec.NameString.StartsWith(".data")).VirtualSize, optHeader.Standard.SizeOfInitializedData);
            Assert.AreEqual(0, optHeader.Standard.SizeOfUninitializedData);
            Assert.AreEqual(DataDirectoryIAT, optHeader.DataDirectories.IAT);
        }

        [Test]
        public void SizeIsCorrect()
        {
            Assert.AreEqual(Marshal.SizeOf<PE32PlusOptionalHeader>(), _packaged.RawData.Length);
        }
    }
}