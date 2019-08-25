using System;
using System.Collections.Generic;
using System.Text;

namespace Llama.PE.Tests.Packaging
{
    using Converters;
    using NUnit.Framework;
    using PE.Packaging.PE32Plus.Idata;
    using PE.Packaging.PE32Plus.MZHeader;

    [TestFixture]
    class MZTests
    {
        class MZInfo : IMZInfo
        {
        }

        private IMZResult _packaged;

        [SetUp]
        public void SetUp()
        {
            var packager = new MZHeaderPackager();
            _packaged = packager.Package(new MZInfo());
        }

        [Test]
        public void NewHeaderOffsetIsPlausible()
        {
            Assert.GreaterOrEqual(_packaged.NewHeaderOffset, _packaged.RawData.Length);
        }

        [Test]
        public void MagicIsMZ()
        {
            Assert.AreEqual("MZ", Encoding.ASCII.GetString(_packaged.RawData.Slice(0, 2)));
        }
    }
}
