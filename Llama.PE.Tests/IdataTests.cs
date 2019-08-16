namespace Llama.PE.Tests
{
    using System;
    using System.IO;
    using NUnit.Framework;

    [TestFixture]
    internal class IdataTests : TestsUsingHeaders
    {
        public IdataTests() : base(File.ReadAllBytes("test.exe")) { }

        [Test]
        public void HintNameTableHasAddsPadding()
        {
            throw new NotImplementedException();
        }
    }
}