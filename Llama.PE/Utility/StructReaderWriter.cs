namespace Llama.PE
{
    using System;
    using System.Reflection.Emit;
    using System.Runtime.InteropServices;
    using Utility;

    public abstract class StructReaderWriter
    {
        protected virtual int RVA { get; set; }

        protected abstract Span<byte> GetMemory(int rva, int size);



        public abstract T Read<T>(int rva = -1) where T : struct
        {
            if (rva == -1)
                rva = RVA;
            else if (rva < 0)
                throw new ArgumentOutOfRangeException(nameof(rva));

            var size = TypeSize<T>.Size;
            RVA = rva + size;
            return MemoryMarshal.Read<T>(GetMemory(rva, size));
        }

        public abstract int Write<T>(T item, int rva = -1) where T : struct
        {
            if (rva == -1)
                rva = RVA;
            else if (rva < 0)
                throw new ArgumentOutOfRangeException(nameof(rva));

            var size = TypeSize<T>.Size;
            MemoryMarshal.Write<T>(GetMemory(rva, size), ref item);
            RVA += size;
            return rva;
        }
    }
}