namespace Llama.BinaryUtils
{
    using System;
    using System.Collections.Generic;

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

        public static T[] ReadArray<T>(this IStructReader reader, int count) where T : struct
        {
            if (count <= 0)
                throw new ArgumentOutOfRangeException(nameof(count));

            var readElements = new T[count];
            for (var i = 0; i < readElements.Length; i++)
                readElements[i] = reader.Read<T>();
            return readElements;
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

        public static ulong WriteArray<T>(this IStructWriter writer, T[] arr, ulong rva) where T : struct
        {
            if (arr == null)
                throw new ArgumentNullException(nameof(arr));
            writer.RVA = rva;
            return WriteArray(writer, arr);
        }

        /// <summary>
        ///     Reads elements of the given type, until default(<see cref="T" />) is read. Leaves the RVA after the nulled element.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="reader"></param>
        /// <returns></returns>
        public static List<T> ReadUntilNull<T>(this IStructReader reader) where T : struct, IEquatable<T>
        {
            var readItems = new List<T>();
            T currentItem;
            while (!(currentItem = reader.Read<T>()).Equals(default))
                readItems.Add(currentItem);
            return readItems;
        }
    }
}