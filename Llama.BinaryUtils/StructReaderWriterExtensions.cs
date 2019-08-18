namespace Llama.BinaryUtils
{
    using System;

    public static class StructReaderWriterExtensions
    {
        public static long WriteArray<T>(this IStructWriter writer, T[] arr, int rva = -1) where T : struct
        {
            if (arr == null)
                throw new ArgumentNullException(nameof(arr));
            if (arr.Length == 0)
                throw new ArgumentException("Value cannot be an empty collection.", nameof(arr));

            var writtenRva = writer.Write(arr[0], rva);
            for (var i = 1; i < arr.Length; i++)
                writer.Write(arr[i]);
            return writtenRva;
        }
    }
}