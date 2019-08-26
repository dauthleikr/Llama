﻿namespace Llama.PE.Tests.Packaging
{
    using System.Text;
    using NUnit.Framework;
    using PE.Packaging.PE32Plus.MZHeader;

    [TestFixture]
    internal class MZHeaderTests
    {
        [SetUp]
        public void SetUp()
        {
            var packager = new MZHeaderPackager();
            _packaged = packager.Package(new MZInfo());
        }

        private class MZInfo : IMZInfo { }

        private IMZResult _packaged;

        [Test]
        public void MagicIsMZ()
        {
            Assert.AreEqual("MZ", Encoding.ASCII.GetString(_packaged.RawData.Slice(0, 2)));
        }

        [Test]
        public void NewHeaderOffsetIsPlausible()
        {
            Assert.GreaterOrEqual(_packaged.NewHeaderOffset, _packaged.RawData.Length);
        }
    }
}