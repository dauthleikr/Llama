namespace Llama.Compiler
{
    using System;
    using Type = Parser.Nodes.Type;

    internal static class Constants
    {
        public const int DummyOffsetInt = int.MaxValue;
        public const long DummyAddress = long.MaxValue;
        public static readonly string HeapHandle = Guid.NewGuid().ToString();
        public static readonly Type LongType = new Type("long");
    }
}