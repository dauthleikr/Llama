namespace Llama.BinaryUtils
{
    using System;
    using System.Runtime.InteropServices;

    public class ArrayStructReaderWriter : IStructReaderWriter
    {
        public ulong RVA { get; set; }
        private readonly byte[] _rawData;

        public ArrayStructReaderWriter(byte[] rawData) => _rawData = rawData;

        public T Read<T>() where T : struct
        {
            var size = TypeSize<T>.Size;
            var span = _rawData.AsSpan((int)RVA, size);
            RVA += (ulong)span.Length;
            return MemoryMarshal.Read<T>(span);
        }

        public ulong Write<T>(T item) where T : struct
        {
            var size = TypeSize<T>.Size;
            var start = RVA;
            var span = _rawData.AsSpan((int)start, size);
            MemoryMarshal.Write(span, ref item);
            RVA += (ulong)span.Length;
            return start;
        }
    }
}