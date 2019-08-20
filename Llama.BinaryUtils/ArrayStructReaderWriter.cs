namespace Llama.BinaryUtils
{
    using System;
    using System.Runtime.InteropServices;

    public class ArrayStructReaderWriter : IStructReaderWriter
    {
        public ulong Offset { get; set; }
        private readonly byte[] _rawData;

        public ArrayStructReaderWriter(byte[] rawData) => _rawData = rawData;

        public T Read<T>() where T : struct
        {
            var size = TypeSize<T>.Size;
            var span = _rawData.AsSpan((int)Offset, size);
            Offset += (ulong)span.Length;
            return MemoryMarshal.Read<T>(span);
        }

        public ulong Write<T>(T item) where T : struct
        {
            var size = TypeSize<T>.Size;
            var start = Offset;
            var span = _rawData.AsSpan((int)start, size);
            MemoryMarshal.Write(span, ref item);
            Offset += (ulong)span.Length;
            return start;
        }
    }
}