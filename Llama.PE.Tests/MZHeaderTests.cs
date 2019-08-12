namespace Llama.PE.Tests
{
    using System;
    using System.IO;
    using System.Reflection.PortableExecutable;
    using NUnit.Framework;

    [TestFixture]
    public unsafe class MZHeaderTests
    {
        [SetUp]
        public void SetUp()
        {
            var exeBytes = File.ReadAllBytes("syncthing.exe");
            if (sizeof(MZHeader) > exeBytes.Length)
                throw new Exception("Bad test exe file");

            fixed (byte* exePtr = exeBytes)
                _header = *(MZHeader*) exePtr;

            var peHeaderBuilder = PEHeaderBuilder.CreateExecutableHeader();
        }

        private MZHeader _header;

        [Test]
        public void MagicIsMZ()
        {
            Assert.AreEqual('M', _header.Magic[0]);
            Assert.AreEqual('Z', _header.Magic[1]);
        }
    }
}