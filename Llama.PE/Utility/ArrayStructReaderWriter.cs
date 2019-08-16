namespace Llama.PE.Utility
{
    using System;
    using System.Runtime.InteropServices;

    public class ArrayStructReaderWriter : IStructReaderWriter
    {
        private readonly byte[] _rawData;
        private long _rva;

        public ArrayStructReaderWriter(byte[] rawData) => _rawData = rawData;

        public T Read<T>(long rva = -1) where T : struct
        {
            if (rva == -1)
                rva = _rva;
            else if (rva < 0)
                throw new ArgumentOutOfRangeException(nameof(rva));

            var size = TypeSize<T>.Size;
            _rva = rva + size;
            return MemoryMarshal.Read<T>(_rawData.AsSpan((int)rva, size));
        }

        public long Write<T>(T item, long rva = -1) where T : struct
        {
            if (rva == -1)
                rva = _rva;
            else if (rva < 0)
                throw new ArgumentOutOfRangeException(nameof(rva));

            var size = TypeSize<T>.Size;
            MemoryMarshal.Write(_rawData.AsSpan((int)rva, size), ref item);
            _rva += size;
            return rva;
        }
    }
}