namespace Llama.Compiler
{
    using System;
    using Type = Parser.Nodes.Type;

    internal static class TypeExtensions
    {
        public static int SizeOf(this Type type)
        {
            switch (type.ChildRelation)
            {
                case Type.WrappingType.None:
                    return SizeOfPrimitiveType(type.PrimitiveType);
                case Type.WrappingType.PointerOf:
                    return 8;
                case Type.WrappingType.ArrayOf:
                    return 8;
                default:
                    throw new ArgumentOutOfRangeException();
            }
        }

        private static int SizeOfPrimitiveType(string type)
        {
            switch (type)
            {
                case "void":
                    return 8;
                case "int":
                    return 4;
                case "long":
                    return 8;
                case "short":
                    return 2;
                case "byte":
                    return 1;
                case "sbyte":
                    return 1;
                case "cstr":
                    return 8;
                case "float":
                    return 4;
                case "double":
                    return 8;
                default:
                    throw new ArgumentOutOfRangeException();
            }
        }
    }
}