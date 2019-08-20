namespace Llama.BinaryUtils
{
    using System;
    using System.Collections.Generic;

    public static class StructReaderWriterExtensions
    {
        public static ulong Write<T>(this IStructWriter writer, T item, ulong offset) where T : struct
        {
            writer.Offset = offset;
            writer.Write(item);
            return offset;
        }

        public static T Read<T>(this IStructReader reader, ulong offset) where T : struct
        {
            reader.Offset = offset;
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

            var writtenOffset = writer.Offset;
            foreach (var item in arr)
                writer.Write(item);
            return writtenOffset;
        }

        public static ulong WriteArray<T>(this IStructWriter writer, T[] arr, ulong offset) where T : struct
        {
            if (arr == null)
                throw new ArgumentNullException(nameof(arr));
            writer.Offset = offset;
            return WriteArray(writer, arr);
        }

        /// <summary>
        ///     Reads elements of the given type, until default(<see cref="T" />) is read. Leaves the offset after the nulled
        ///     element.
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