namespace Llama.PE
{
    using System;
    using BinaryUtils;

    public static class StructWriterExtensions
    {
        //public static void Write<T>(this IStructWriter writer, T item, IPEReader<T> itemReader)
        //{
        //    if (writer == null)
        //        throw new ArgumentNullException(nameof(writer));
        //    if (item == null)
        //        throw new ArgumentNullException(nameof(item));
        //    if (itemReader == null)
        //        throw new ArgumentNullException(nameof(itemReader));

        //    itemReader.Write(item, writer);
        //}

        //public static void WriteArray<T>(this IStructWriter writer, T[] items, IPEReader<T> itemMapper)
        //{
        //    if (writer == null)
        //        throw new ArgumentNullException(nameof(writer));
        //    if (items == null)
        //        throw new ArgumentNullException(nameof(items));
        //    if (itemMapper == null)
        //        throw new ArgumentNullException(nameof(itemMapper));

        //    foreach (var item in items)
        //        itemMapper.Write(item, writer);
        //}

        public static T[] ReadArray<T>(this IStructReader reader, IPEReader<T> itemMapper, IPE32PlusContext image, int count)
        {
            if (reader == null)
                throw new ArgumentNullException(nameof(reader));
            if (itemMapper == null)
                throw new ArgumentNullException(nameof(itemMapper));
            if (count <= 0)
                throw new ArgumentOutOfRangeException(nameof(count));

            var readItems = new T[count];
            for (var i = 0; i < readItems.Length; i++)
                readItems[i] = itemMapper.Read(reader, image);
            return readItems;
        }
    }
}