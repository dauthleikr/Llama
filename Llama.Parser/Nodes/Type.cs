namespace Llama.Parser.Nodes
{
    using System;
    using System.ComponentModel;

    public class Type : IEquatable<Type>
    {
        public enum WrappingType
        {
            None,
            PointerOf,
            ArrayOf
        }

        public string PrimitiveType { get; }
        public WrappingType ChildRelation { get; }
        public Type Child { get; }

        public Type(string primitiveType) => PrimitiveType = primitiveType;

        public Type(Type underlying, WrappingType wrappingType)
        {
            if (!Enum.IsDefined(typeof(WrappingType), wrappingType) || wrappingType == WrappingType.None)
                throw new InvalidEnumArgumentException(nameof(wrappingType), (int)wrappingType, typeof(WrappingType));

            Child = underlying ?? throw new ArgumentNullException(nameof(underlying));
            ChildRelation = wrappingType;
        }

        public override string ToString() =>
            ChildRelation switch
            {
                WrappingType.None => PrimitiveType,
                WrappingType.ArrayOf => $"{Child}[]",
                WrappingType.PointerOf => $"{Child}*",
                _ => throw new ArgumentOutOfRangeException(ChildRelation.ToString())
            };

        public bool Equals(Type other)
        {
            if (ReferenceEquals(null, other))
                return false;
            if (ReferenceEquals(this, other))
                return true;
            return PrimitiveType == other.PrimitiveType && ChildRelation == other.ChildRelation && Equals(Child, other.Child);
        }

        public override bool Equals(object obj)
        {
            if (ReferenceEquals(null, obj))
                return false;
            if (ReferenceEquals(this, obj))
                return true;
            if (obj.GetType() != this.GetType())
                return false;
            return Equals((Type)obj);
        }

        public override int GetHashCode()
        {
            unchecked
            {
                var hashCode = (PrimitiveType != null ? PrimitiveType.GetHashCode() : 0);
                hashCode = (hashCode * 397) ^ (int)ChildRelation;
                hashCode = (hashCode * 397) ^ (Child != null ? Child.GetHashCode() : 0);
                return hashCode;
            }
        }

        public static bool operator ==(Type left, Type right) => Equals(left, right);

        public static bool operator !=(Type left, Type right) => !Equals(left, right);
    }
}