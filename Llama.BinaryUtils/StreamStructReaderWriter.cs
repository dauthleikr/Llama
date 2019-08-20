namespace Llama.BinaryUtils
{
    using System;
    using System.IO;
    using System.Runtime.InteropServices;

    public class StreamStructReaderWriter : IStructReaderWriter
    {
        public ulong RVA
        {
            get => _rva;
            set
            {
                if (value > long.MaxValue)
                    throw new ArgumentOutOfRangeException(nameof(value));

                _rva = value;
                _stream.Position = _initialPosition + (long)value;
            }
        }

        private ulong _rva;
        private readonly long _initialPosition;
        private readonly Stream _stream;

        public StreamStructReaderWriter(Stream stream)
        {
            if (!stream.CanSeek)
                throw new ArgumentException("Stream needs to be seekable", nameof(stream));
            _stream = stream;
            _initialPosition = stream.Position;
        }

        public T Read<T>() where T : struct
        {
            var result = default(T);
            var spanStruct = MemoryMarshal.CreateSpan(ref result, 1);
            var spanBytes = MemoryMarshal.AsBytes(spanStruct);
            _rva += (ulong)_stream.Read(spanBytes);
            return result;
        }

        public ulong Write<T>(T item) where T : struct
        {
            var span = MemoryMarshal.CreateReadOnlySpan(ref item, 1);
            var spanBytes = MemoryMarshal.AsBytes(span);
            var start = _rva;
            _stream.Write(spanBytes);
            _rva += (ulong)spanBytes.Length;
            return start;
        }
    }
}