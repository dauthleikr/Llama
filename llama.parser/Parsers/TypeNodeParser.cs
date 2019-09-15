namespace Llama.Parser.Parsers
{
    using Lexer;
    using Nodes;

    internal class TypeNodeParser : IParse<TypeNode>
    {
        public TypeNode Read(IParseContext context)
        {
            var type = new TypeNode(context.ReadOrPanic(TokenKind.PrimitiveType).RawText);

            while (true)
                switch (context.NextCodeToken.Kind)
                {
                    case TokenKind.Pointer:
                        context.ReadCodeToken();
                        type = new TypeNode(type, TypeNode.WrappingType.PointerOf);
                        break;
                    case TokenKind.OpenSquareBracket:
                        context.ReadCodeToken();
                        context.ReadOrPanic(TokenKind.CloseSquareBracket);
                        type = new TypeNode(type, TypeNode.WrappingType.ArrayOf);
                        break;
                    default:
                        return type;
                }
        }
    }
}