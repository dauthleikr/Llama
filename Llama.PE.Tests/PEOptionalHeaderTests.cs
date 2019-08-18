namespace Llama.PE.Tests
{
    using System.IO;
    using Header;
    using NUnit.Framework;

    [TestFixture]
    public class PEOptionalHeaderTests : TestsUsingHeaders
    {
        public PEOptionalHeaderTests() : base(File.ReadAllBytes("test.exe")) { }

        [Test]
        public void ExecutableKindIsValid()
        {
            Assert.That(
                OptionalHeader.Standard.ExecutableKind == ExecutableKind.ROM ||
                OptionalHeader.Standard.ExecutableKind == ExecutableKind.PE32 ||
                OptionalHeader.Standard.ExecutableKind == ExecutableKind.PE32Plus,
                "Bad executable kind (headers corrupt?)"
            );
        }
    }
}