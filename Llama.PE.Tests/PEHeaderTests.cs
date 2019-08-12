namespace Llama.PE.Tests
{
    using System;
    using System.IO;
    using NUnit.Framework;

    [TestFixture]
    public unsafe class PEHeaderTests
    {
        [SetUp]
        public void SetUp()
        {
            var exeBytes = File.ReadAllBytes("test.exe");
            if (sizeof(MZHeader) > exeBytes.Length)
                throw new Exception("Bad test exe file");

            fixed (byte* exePtr = exeBytes)
            {
                var mzheader = *(MZHeader*) exePtr;
                if (sizeof(PEHeader) + mzheader.NewHeaderRVA > exeBytes.Length)
                    throw new Exception("Bad test exe file (cannot fit PE header)");
                _header = *(PEHeader*) (exePtr + mzheader.NewHeaderRVA);
            }
        }

        private PEHeader _header;

        [Test]
        public void HeaderHasExpectedSize()
        {
            Assert.AreEqual(sizeof(PEHeader), 4 + 20);
        }

        [Test]
        public void MagicIsPE()
        {
            Assert.AreEqual('P', _header.Magic[0]);
            Assert.AreEqual('E', _header.Magic[1]);
            Assert.AreEqual('\0', _header.Magic[2]);
            Assert.AreEqual('\0', _header.Magic[3]);
        }
    }
}