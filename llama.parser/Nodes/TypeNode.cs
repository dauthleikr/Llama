namespace Llama.Parser.Nodes
{
    using System;
    using System.ComponentModel;
    using Lexer;

    internal class TypeNode
    {
        public enum WrappingType
        {
            None,
            PointerOf,
            ArrayOf
        }

        public Token PrimitiveType { get; }
        public WrappingType ChildRelation { get; }
        public TypeNode Child { get; }

        public TypeNode(Token primitiveType)
        {
            if (primitiveType.Kind != TokenKind.PrimitiveType)
                throw new ArgumentException($"{nameof(primitiveType)} has to be of kind: {nameof(TokenKind.PrimitiveType)}");

            PrimitiveType = primitiveType;
        }

        public TypeNode(TypeNode underlying, WrappingType wrappingType)
        {
            if (!Enum.IsDefined(typeof(WrappingType), wrappingType) || wrappingType == WrappingType.None)
                throw new InvalidEnumArgumentException(nameof(wrappingType), (int)wrappingType, typeof(WrappingType));

            Child = underlying ?? throw new ArgumentNullException(nameof(underlying));
            ChildRelation = wrappingType;
        }
    }
}