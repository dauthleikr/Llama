namespace Llama.BinaryUtils
{
    using System;
    using System.Reflection.Emit;

    /// <summary>
    ///     https://stackoverflow.com/a/42437504/6119085
    /// </summary>
    public static class TypeSize<T>
    {
        public static readonly int Size;

        static TypeSize()
        {
            var dm = new DynamicMethod("SizeOfType", typeof(int), new Type[] { });
            var il = dm.GetILGenerator();
            il.Emit(OpCodes.Sizeof, typeof(T));
            il.Emit(OpCodes.Ret);
            Size = (int)dm.Invoke(null, null);
        }
    }
}