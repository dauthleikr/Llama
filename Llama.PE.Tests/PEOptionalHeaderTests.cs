namespace Llama.PE.Tests
{
    using System;
    using System.IO;
    using NUnit.Framework;

    [TestFixture]
    public unsafe class PEOptionalHeaderTests
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
                if (sizeof(PEHeader) + sizeof(PEOptionalHeader) + mzheader.NewHeaderRVA > exeBytes.Length)
                    throw new Exception("Bad test exe file (cannot fit PE optional header)");
                _header = *(PEOptionalHeader*) (exePtr + mzheader.NewHeaderRVA + sizeof(PEHeader));
            }
        }

        private PEOptionalHeader _header;

        [Test]
        public void ExecutableKindIsValid()
        {
            Assert.That(_header.ExecutableKind == ExecutableKind.ROM ||
                        _header.ExecutableKind == ExecutableKind.PE32 ||
                        _header.ExecutableKind == ExecutableKind.PE32Plus);
        }
    }
}