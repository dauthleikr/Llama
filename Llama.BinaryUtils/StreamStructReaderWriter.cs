namespace Llama.BinaryUtils
{
    using System;
    using System.IO;
    using System.Runtime.InteropServices;

    public class StreamStructReaderWriter : IStructReaderWriter
    {
        private readonly long _initialPosition;
        private readonly Stream _stream;
        private long _rva;

        public StreamStructReaderWriter(Stream stream)
        {
            if (!stream.CanSeek)
                throw new ArgumentException("Stream needs to be seekable", nameof(stream));
            _stream = stream;
            _initialPosition = stream.Position;
        }

        public T Read<T>(long rva = -1) where T : struct
        {
            if (rva == -1)
                rva = _rva;
            else if (rva < 0)
                throw new ArgumentOutOfRangeException(nameof(rva));

            _stream.Position = _initialPosition + rva;
            var result = default(T);
            var spanStruct = MemoryMarshal.CreateSpan(ref result, 1);
            _stream.Read(MemoryMarshal.AsBytes(spanStruct));
            _rva = _stream.Position;
            return result;
        }

        public long Write<T>(T item, long rva = -1) where T : struct
        {
            if (rva == -1)
                rva = _rva;
            else if (rva < 0)
                throw new ArgumentOutOfRangeException(nameof(rva));

            var startPosition = rva;
            _stream.Position = _initialPosition + rva;
            var span = MemoryMarshal.CreateReadOnlySpan(ref item, 1);
            _stream.Write(MemoryMarshal.AsBytes(span));
            return startPosition;
        }
    }
}