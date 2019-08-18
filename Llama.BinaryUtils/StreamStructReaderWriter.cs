namespace Llama.BinaryUtils
{
    using System;
    using System.IO;
    using System.Runtime.InteropServices;

    public class StreamStructReaderWriter : IStructReaderWriter
    {
        private readonly long _initialPosition;
        private readonly Stream _stream;
        public long RVA { get; set; }

        public StreamStructReaderWriter(Stream stream)
        {
            if (!stream.CanSeek)
                throw new ArgumentException("Stream needs to be seekable", nameof(stream));
            _stream = stream;
            _initialPosition = stream.Position;
        }

        public T Read<T>(long rva = -1) where T : struct
        {
            if (rva < -1)
                throw new ArgumentOutOfRangeException(nameof(rva));
            if (rva >= 0)
            {
                _stream.Position = _initialPosition + rva;
                RVA = rva;
            }

            var result = default(T);
            var spanStruct = MemoryMarshal.CreateSpan(ref result, 1);
            var spanBytes = MemoryMarshal.AsBytes(spanStruct);
            RVA += _stream.Read(spanBytes);
            return result;
        }

        public long Write<T>(T item, long rva = -1) where T : struct
        {
            if (rva == -1)
                rva = RVA;
            else if (rva < 0)
                throw new ArgumentOutOfRangeException(nameof(rva));
            else
            {
                _stream.Position = _initialPosition + rva;
                RVA = rva;
            }

            var span = MemoryMarshal.CreateReadOnlySpan(ref item, 1);
            var spanBytes = MemoryMarshal.AsBytes(span);
            _stream.Write(spanBytes);
            RVA += spanBytes.Length;
            return rva;
        }
    }
}