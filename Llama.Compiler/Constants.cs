namespace Llama.Compiler
{
    using System;

    internal static class Constants
    {
        public const int DummyOffsetInt = int.MaxValue;
        public const long DummyAddress = long.MaxValue;
        public static readonly string HeapHandle = Guid.NewGuid().ToString();
    }
}