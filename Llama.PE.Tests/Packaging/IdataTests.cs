using System;
using System.Collections.Generic;
using System.Text;

namespace Llama.PE.Tests.Packaging
{
    using Converters;
    using NUnit.Framework;
    using PE.Packaging.PE32Plus.Idata;

    [TestFixture]
    class IdataTests
    {
        class IdataInfo : IIdataInfo
        {
            public IEnumerable<(string library, string function)> Imports => new (string library, string function)[]
            {
                ("kernel32.dll", "WriteFile"),
                ("kernel32.dll", "VirtualAllocEx"),
                ("user32.dll", "MessageBox")
            };

            public uint IdataRVA => 10000;
            public uint IATBlockSize => 32;
        }

        private IIdataResult _packaged;

        [SetUp]
        public void SetUp()
        {
            var packager = new IdataSectionPackager(new HintNameEntryWriter());
            _packaged = packager.Package(new IdataInfo());
        }

    }
}
