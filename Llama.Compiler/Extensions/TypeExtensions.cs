namespace Llama.Compiler.Extensions
{
    using System;
    using System.Linq;
    using spit;
    using Type = Parser.Nodes.Type;

    internal static class TypeExtensions
    {
        private static readonly Register[] VolatileIntRegisters =
        {
            Register64.RAX,
            Register64.RCX,
            Register64.RDX,
            Register64.R8,
            Register64.R9,
            Register64.R10,
            Register64.R11
        };

        private static readonly Register[] VolatileFloatRegisters =
        {
            XmmRegister.XMM0,
            XmmRegister.XMM1,
            XmmRegister.XMM2,
            XmmRegister.XMM3,
            XmmRegister.XMM4,
            XmmRegister.XMM5
        };

        public static bool IsSignedInteger(this Type type)
        {
            if (type.ChildRelation != Type.WrappingType.None)
                return false;

            switch (type.PrimitiveType)
            {
                case "long":
                case "int":
                case "short":
                case "sbyte":
                    return true;
            }

            return false;
        }

        /// <summary>
        ///     Returns if a value of this type should be stored in general purpose (int) registers, or SSE2 registers (e.g. float,
        ///     double)
        /// </summary>
        /// <param name="type"></param>
        /// <returns></returns>
        public static bool IsIntegerRegisterType(this Type type)
        {
            if (type.ChildRelation != Type.WrappingType.None)
                return true;

            switch (type.PrimitiveType)
            {
                case "float":
                case "double":
                    return false;
            }

            return true;
        }

        private static bool CanPromote(string targetType, string sourceType)
        {
            switch (sourceType)
            {
                case "int" when targetType == "long":
                case "short" when targetType == "int" || targetType == "long":
                case "sbyte" when targetType == "short" || targetType == "int" || targetType == "long":
                    return true;
                default:
                    return false;
            }
        }

        public static Register MakeRegisterWithCorrectSize(this Type type, Register register)
        {
            if (register.FloatingPoint)
                return register;
            return MakeRegisterWithCorrectSize(type, register.AsR64(), default);
        }

        public static Register MakeRegisterWithCorrectSize(this Type type, Register64 intRegister, XmmRegister floatRegister)
        {
            if (!type.IsIntegerRegisterType())
                return floatRegister;

            switch (type.SizeOf())
            {
                case 8:
                    return intRegister;
                case 4:
                    return (Register32)intRegister;
                case 2:
                    return (Register16)intRegister;
                case 1:
                    return (Register8)intRegister;
                default:
                    throw new NotImplementedException($"Proper register for type {type} (size {type.SizeOf()}) has not been implemented yet");
            }
        }

        public static bool CanAssignImplicitly(this Type type, Type source)
        {
            if (type.ChildRelation != source.ChildRelation) // cannot do weird stuff implicitly
                return false;

            if (type.ChildRelation == Type.WrappingType.None) // if they are the same type, or can be promoted to the same type, we are good
                return type.PrimitiveType == source.PrimitiveType || CanPromote(type.PrimitiveType, source.PrimitiveType);

            return CanAssignImplicitly(type.Child, source.Child); // unwrap pointer/array/... and check type underneath
        }

        public static void AssertCanAssignImplicitly(this Type type, Type other)
        {
            if (!type.CanAssignImplicitly(other))
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
                case "bool":
                    return 1;
                default:
                    throw new ArgumentOutOfRangeException();
            }
        }

        public static Register OtherVolatileIntRegister(this Type targetType, params Register[] occupiedRegisters)
        {
            if (targetType == null)
                throw new ArgumentNullException(nameof(targetType));
            if (occupiedRegisters.Length >= VolatileIntRegisters.Length)
                throw new ArgumentException("No registers left", nameof(occupiedRegisters));

            return Register.IntRegisterFromSize(
                targetType.SizeOf(),
                VolatileIntRegisters.First(vol => occupiedRegisters.All(occ => !occ.IsSameRegister(vol))).FullValue
            );
        }

        public static Register OtherVolatileFloatRegister(this Type targetType, params Register[] occupiedRegisters)
        {
            if (targetType == null)
                throw new ArgumentNullException(nameof(targetType));
            if (occupiedRegisters.Length >= VolatileFloatRegisters.Length)
                throw new ArgumentException("No registers left", nameof(occupiedRegisters));

            return VolatileFloatRegisters.First(vol => occupiedRegisters.All(occ => !occ.IsSameRegister(vol)));
        }
    }
}