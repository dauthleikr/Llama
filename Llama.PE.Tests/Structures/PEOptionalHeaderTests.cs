﻿namespace Llama.PE.Tests.Structures
{
    using System.IO;
    using NUnit.Framework;
    using PE.Structures.Header;

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