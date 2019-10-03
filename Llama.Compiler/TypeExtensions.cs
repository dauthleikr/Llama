namespace Llama.Compiler
{
    using System;
    using Type = Parser.Nodes.Type;

    internal static class TypeExtensions
    {
        public static bool IsIntegerType(this Type type)
        {
            if (type.ChildRelation == Type.WrappingType.PointerOf)
                return true;
            if (type.ChildRelation == Type.WrappingType.ArrayOf)
                return false;

            switch (type.PrimitiveType)
            {
                case "long":
                case "int":
                case "short":
                case "sbyte":
                case "byte":
                case "cstr":
                    return true;
            }
            return false;
        }

        public static bool CanAssign(this Type type, Type other)
        {
            if (type.ChildRelation != other.ChildRelation) // cannot do weird stuff implicitly
                return false;

            if (type.ChildRelation == Type.WrappingType.None) // if they are the same type, or can be promoted to the same type, we are good
                return type.PrimitiveType == other.PrimitiveType || CanPromote(type.PrimitiveType, other.PrimitiveType);

            return CanAssign(type.Child, other.Child); // unwrap pointer/array/... and check type underneath
        }

        private static bool CanPromote(string targetType, string sourceType)
        {
            switch (sourceType)
            {
                case "int" when targetType == "long":
                case "short" when targetType == "int" || targetType == "long":
                case "byte" when targetType == "short" || targetType == "int" || targetType == "long":
                    return true;
                default:
                    return false;
            }
        }

        public static void AssertCanAssign(this Type type, Type other)
        {
            if (!type.CanAssign(other))
                throw new TypeMismatchException(type.ToString(), other.ToString());
        }

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