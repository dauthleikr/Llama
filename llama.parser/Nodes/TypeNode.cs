namespace Llama.Parser.Nodes
{
    using System;
    using System.ComponentModel;

    internal class TypeNode
    {
        public enum WrappingType
        {
            None,
            PointerOf,
            ArrayOf
        }

        public string PrimitiveType { get; }
        public WrappingType ChildRelation { get; }
        public TypeNode Child { get; }

        public TypeNode(string primitiveType) => PrimitiveType = primitiveType;

        public TypeNode(TypeNode underlying, WrappingType wrappingType)
        {
            if (!Enum.IsDefined(typeof(WrappingType), wrappingType) || wrappingType == WrappingType.None)
                throw new InvalidEnumArgumentException(nameof(wrappingType), (int)wrappingType, typeof(WrappingType));

            Child = underlying ?? throw new ArgumentNullException(nameof(underlying));
            ChildRelation = wrappingType;
        }
    }
}