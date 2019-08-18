namespace Llama.BinaryUtils.Tests
{
    using System;
    using System.Diagnostics;
    using System.IO;
    using NUnit.Framework;

    [TestFixture]
    internal class StreamStructReaderWriterTests
    {
        private readonly TestStruct1 _testStruct1 = new TestStruct1 { Value = 123 };
        private readonly TestStruct2 _testStruct2 = new TestStruct2 { Value = 222, Value2 = 22 };

        private void CanWriteAndRead<T>(T testValue) where T : struct
        {
            var stream = new MemoryStream();
            var rw = new StreamStructReaderWriter(stream);

            rw.Write(testValue);
            var readStruct = rw.Read<T>(0);

            Assert.AreEqual(testValue, readStruct, $"Can not read struct of type {typeof(T).Name} after writing it");
        }

        [Test]
        public void CanWriteAndRead()
        {
            CanWriteAndRead(true);
            CanWriteAndRead(123);
            CanWriteAndRead(123);
            CanWriteAndRead(_testStruct1);
            CanWriteAndRead(_testStruct2);
        }

        [Test]
        public void KeepsInitialRVA()
        {
            var stream = new MemoryStream();
            stream.WriteByte(2);
            stream.WriteByte(1);
            stream.WriteByte(0);

            var rw = new StreamStructReaderWriter(stream);
            var read = rw.Read<TestStruct1>(rw.Write(_testStruct1));
            var read2 = rw.Read<TestStruct2>(rw.Write(_testStruct2));

            Assert.AreEqual(_testStruct1, read, $"Can not read struct of type {nameof(TestStruct1)} correctly after writing it");
            Assert.AreEqual(_testStruct2, read2, $"Can not read struct of type {nameof(TestStruct2)} correctly after writing it");
        }

        [Test]
        public void IncreasesRVA()
        {
            var stream = new MemoryStream();
            var rw = new StreamStructReaderWriter(stream);

            rw.Write(123);
            rw.Write('c');
            rw.Write(true);

            Assert.AreEqual(7, stream.Length);
        }

        [Test]
        public void IsCorrectSize()
        {
            var stream = new MemoryStream();
            var rw = new StreamStructReaderWriter(stream);

            rw.Write(_testStruct2);

            Assert.AreEqual(9, stream.Length);
        }
    }
}