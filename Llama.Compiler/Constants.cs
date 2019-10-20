namespace Llama.Compiler
{
    using System;
    using Type = Parser.Nodes.Type;

    internal static class Constants
    {
        public const int DummyOffsetInt = int.MaxValue;
        public const long DummyAddress = long.MaxValue;
        public static readonly string HeapHandleIdentifier = Guid.NewGuid().ToString();
        public static readonly Type LongType = new Type("long");
        public static readonly Type IntType = new Type("int");
        public static readonly Type ShortType = new Type("short");
        public static readonly Type SbyteType = new Type("sbyte");
        public static readonly Type ByteType = new Type("byte");
        public static readonly Type CstrType = new Type("cstr");
        public static readonly Type DoubleType = new Type("double");
        public static readonly Type FloatType = new Type("float");
        public static readonly Type FunctionPointerType = new Type("!FunctionPointer");
        public static readonly Type BoolType = new Type("bool");
    }
}