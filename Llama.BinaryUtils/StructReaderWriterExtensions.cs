namespace Llama.BinaryUtils
{
    using System;

    public static class StructReaderWriterExtensions
    {
        public static ulong Write<T>(this IStructWriter writer, T item, ulong rva) where T : struct
        {
            writer.RVA = rva;
            writer.Write(item);
            return rva;
        }

        public static T Read<T>(this IStructReader reader, ulong rva) where T : struct
        {
            reader.RVA = rva;
            return reader.Read<T>();
        }

        public static ulong WriteArray<T>(this IStructWriter writer, T[] arr) where T : struct
        {
            if (arr == null)
                throw new ArgumentNullException(nameof(arr));

            var writtenRva = writer.RVA;
            foreach (var item in arr)
                writer.Write(item);
            return writtenRva;
        }
    }
}