namespace Llama.BinaryUtils.Tests
{
    using NUnit.Framework;

    [TestFixture]
    internal class ArrayStructReaderWriterTests
    {
        private readonly TestStruct1 testStruct1 = new TestStruct1 {Value = 123};
        private readonly TestStruct2 testStruct2 = new TestStruct2 {Value = 22, Value2 = 222};

        private void CanWriteAndRead<T>(T testValue) where T : struct
        {
            var rw = new ArrayStructReaderWriter(new byte[TypeSize<T>.Size]);

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
            CanWriteAndRead(testStruct1);
            CanWriteAndRead(testStruct2);
        }

        [Test]
        public void HandlesRVACorrectly()
        {
            var rw = new ArrayStructReaderWriter(new byte[TypeSize<TestStruct1>.Size + TypeSize<TestStruct2>.Size]);

            rw.Write(testStruct1);
            rw.Write(testStruct2);

            var read = rw.Read<TestStruct1>(0);
            var read2 = rw.Read<TestStruct2>();

            Assert.AreEqual(testStruct1, read, $"Can not read struct of type {nameof(TestStruct1)} correctly after writing it");
            Assert.AreEqual(testStruct2, read2, $"Can not read struct of type {nameof(TestStruct2)} correctly after writing it");
        }

        [Test]
        public void IsCorrectSize()
        {
            var rwGood = new ArrayStructReaderWriter(new byte[9]);
            var rwBad = new ArrayStructReaderWriter(new byte[8]);

            rwGood.Write(testStruct2);

            Assert.Catch(() => rwBad.Write(testStruct2));
        }
    }
}