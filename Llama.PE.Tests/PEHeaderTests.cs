namespace Llama.PE.Tests
{
    using System;
    using System.IO;
    using System.Text;
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
                var mzheader = *(MZHeader*)exePtr;
                if (sizeof(PEHeader) + mzheader.NewHeaderRVA > exeBytes.Length)
                    throw new Exception("Bad test exe file (cannot fit PE header)");
                _header = *(PEHeader*)(exePtr + mzheader.NewHeaderRVA);
            }
        }

        private PEHeader _header;

        [Test]
        public void HeaderHasExpectedSize()
        {
            Assert.AreEqual(sizeof(PEHeader), 4 + 20, "Header size does not match standard");
        }

        [Test]
        public void MagicIsPE()
        {
            fixed (byte* ptr = _header.Magic)
                Assert.AreEqual("PE\0\0", Encoding.ASCII.GetString(ptr, 4), "Magic value not matching (headers corrupt?)");
        }
    }
}