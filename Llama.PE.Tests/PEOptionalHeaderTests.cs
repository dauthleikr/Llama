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
                if (sizeof(PEHeader) + sizeof(PE32PlusOptionalHeader) + mzheader.NewHeaderRVA > exeBytes.Length)
                    throw new Exception("Bad test exe file (cannot fit PE optional header)");
                _header = *(PE32PlusOptionalHeader*) (exePtr + mzheader.NewHeaderRVA + sizeof(PEHeader));
            }
        }

        private PE32PlusOptionalHeader _header;

        [Test]
        public void ExecutableKindIsValid()
        {
            Assert.That(_header.Standard.ExecutableKind == ExecutableKind.ROM ||
                        _header.Standard.ExecutableKind == ExecutableKind.PE32 ||
                        _header.Standard.ExecutableKind == ExecutableKind.PE32Plus);
        }
    }
}