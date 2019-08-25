using System;
using System.Collections.Generic;
using System.Text;

namespace Llama.BinaryUtils
{
    using System.Runtime.InteropServices;

    public static class StructConverter
    {
        public static byte[] GetBytes<T>(T item) where T : struct
        {
            var size = TypeSize<T>.Size;
            var data = new byte[size];
            MemoryMarshal.Write(data.AsSpan(), ref item);
            return data;
        }

        public static T GetStruct<T>(byte[] data) where T : struct => MemoryMarshal.Read<T>(data.AsSpan());
    }
}
