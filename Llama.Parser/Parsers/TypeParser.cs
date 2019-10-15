namespace Llama.Parser.Parsers
{
    using Lexer;
    using Nodes;

    internal class TypeParser : IParse<Type>
    {
        public Type Read(IParseContext context)
        {
            var type = new Type(context.ReadOrPanic(TokenKind.PrimitiveType).RawText);

            while (true)
                switch (context.NextCodeToken.Kind)
                {
                    case TokenKind.Pointer:
                        context.ReadCodeToken();
                        type = new Type(type, Type.WrappingType.PointerOf);
                        break;
                    case TokenKind.OpenSquareBracket:
                        context.ReadCodeToken();
                        context.ReadOrPanic(TokenKind.CloseSquareBracket);
                        type = new Type(type, Type.WrappingType.ArrayOf);
                        break;
                    default:
                        return type;
                }
        }
    }
}