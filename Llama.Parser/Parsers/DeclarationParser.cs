namespace Llama.Parser.Parsers
{
    using Lexer;
    using Nodes;

    internal class DeclarationParser : IParse<Declaration>
    {
        public Declaration Read(IParseContext context)
        {
            var type = context.ReadNode<Type>();
            var name = context.ReadOrPanic(TokenKind.Identifier);

            if (context.NextCodeToken.Kind == TokenKind.Assignment)
            {
                context.ReadOrPanic(TokenKind.Assignment);
                return new Declaration(type, name, context.ReadNode<IExpression>());
            }

            return new Declaration(type, name);
        }
    }
}