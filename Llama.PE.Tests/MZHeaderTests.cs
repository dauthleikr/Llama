namespace Llama.PE.Tests
{
    using System;
    using System.IO;
    using System.Reflection.PortableExecutable;
    using System.Text;
    using NUnit.Framework;

    [TestFixture]
    public unsafe class MZHeaderTests
    {
        [SetUp]
        public void SetUp()
        {
            var exeBytes = File.ReadAllBytes("syncthing.exe");
            _testFileSize = exeBytes.Length;
            if (sizeof(MZHeader) > exeBytes.Length)
                throw new Exception("Bad test exe file");

            fixed (byte* exePtr = exeBytes)
                _header = *(MZHeader*)exePtr;
        }

        private MZHeader _header;
        private int _testFileSize;

        [Test]
        public void MagicIsMZ()
        {
            fixed (byte* ptr = _header.Magic)
                Assert.AreEqual("MZ", Encoding.ASCII.GetString(ptr, 2), "Magic value not matching (headers corrupt?)");
        }

        [Test]
        public void HasPlausiblePEHeaderOffset()
        {
            Assert.That(_header.NewHeaderRVA, Is.InRange(sizeof(MZHeader), _testFileSize));
        }
    }
}