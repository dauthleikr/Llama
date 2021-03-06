﻿namespace Llama.Parser
{
    using Lexer;

    internal static class ParseContextExtensions
    {
        public static Token ReadOrPanic(this IParseContext context, TokenKind kind)
        {
            if (context.NextCodeToken.Kind != kind)
            {
                context.Panic($"Expected token of kind: {kind}, but got: {context.NextCodeToken}");
                return default;
            }

            return context.ReadCodeToken();
        }
    }
}