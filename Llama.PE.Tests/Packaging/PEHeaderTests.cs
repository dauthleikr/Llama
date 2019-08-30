namespace Llama.PE.Tests.Packaging
{
    using System;
    using System.Reflection.PortableExecutable;
    using System.Runtime.InteropServices;
    using System.Text;
    using BinaryUtils;
    using Moq;
    using NUnit.Framework;
    using PE.Packaging.PE32Plus.PEHeader;
    using PEHeader = PE.Structures.Header.PEHeader;

    [TestFixture]
    internal class PEHeaderTests
    {
        [OneTimeSetUp]
        public void SetUp()
        {
            var info = new Mock<IPEInfo>();
            info.Setup(item => item.Characteristics).Returns(Characteristics);
            info.Setup(item => item.Architecture).Returns(Architecture);
            info.Setup(item => item.NumberOfSections).Returns(NumberOfSections);
            info.Setup(item => item.TimeStamp).Returns(Timestamp);

            var packager = new PEHeaderPackager();
            _packaged = packager.Package(info.Object);
        }

        private IPEResult _packaged;

        // ReSharper disable once BitwiseOperatorOnEnumWithoutFlags
        private const Characteristics Characteristics = System.Reflection.PortableExecutable.Characteristics.ExecutableImage |
                                                        System.Reflection.PortableExecutable.Characteristics.LargeAddressAware;

        private const Machine Architecture = Machine.Amd64;
        private const ushort NumberOfSections = 2;
        private static readonly DateTime Timestamp = DateTime.Now;

        [Test]
        public void CanReadProperties()
        {
            var reader = new ArrayStructReaderWriter(_packaged.RawData);
            var peHeader = reader.Read<PEHeader>();
            var timeStampFromHeader = DateTimeOffset.FromUnixTimeSeconds(peHeader.CreationTimePOSIX).DateTime;

            Assert.AreEqual(Characteristics, peHeader.Characteristics);
            Assert.AreEqual(Architecture, peHeader.Architecture);
            Assert.AreEqual(NumberOfSections, peHeader.NumberOfSections);
            Assert.That(() => timeStampFromHeader - Timestamp, Is.LessThan(TimeSpan.FromSeconds(1)));
        }

        [Test]
        public void MagicIsPE()
        {
            Assert.AreEqual("PE\0\0", Encoding.ASCII.GetString(_packaged.RawData, 0, 4));
        }

        [Test]
        public void SizeIsCorrect()
        {
            Assert.AreEqual(Marshal.SizeOf<PEHeader>(), _packaged.RawData.Length);
        }
    }
}