namespace Llama.Parser.Parsers
{
    using System.Collections.Generic;
    using Lexer;
    using Nodes;

    internal class FunctionDeclarationParser : IParse<FunctionDeclaration>
    {
        public FunctionDeclaration Read(IParseContext context)
        {
            var returnType = context.ReadNode<Type>();
            var identifier = context.ReadOrPanic(TokenKind.Identifier);
            context.ReadOrPanic(TokenKind.OpenParanthesis);
            var parameters = new List<FunctionDeclaration.Parameter>();
            while (context.NextCodeToken.Kind != TokenKind.CloseParanthesis)
            {
                var parameterType = context.ReadNode<Type>();
                var parameterIdentifier = context.ReadOrPanic(TokenKind.Identifier);
                if (context.NextCodeToken.Kind == TokenKind.Comma)
                    context.ReadCodeToken();
                parameters.Add(new FunctionDeclaration.Parameter(parameterType, parameterIdentifier));
            }

            context.ReadOrPanic(TokenKind.CloseParanthesis);
            return new FunctionDeclaration(returnType, identifier, parameters.ToArray());
        }
    }
}