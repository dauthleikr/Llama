namespace Llama.Parser.Parsers
{
    using System.Collections.Generic;
    using Lexer;
    using Nodes;

    internal class StatementParser : IParse<IStatement>
    {
        public IStatement Read(IParseContext context)
        {
            return context.NextCodeToken.Kind switch
            {
                TokenKind.If => ReadIf(context),
                TokenKind.While => ReadWhile(context),
                TokenKind.For => ReadFor(context),
                TokenKind.Return => ReadReturn(context),
                TokenKind.PrimitiveType => ReadDeclaration(context),
                TokenKind.OpenBraces => ReadBlock(context),
                _ => ReadExpression(context)
            };
        }

        private static IExpression ReadExpression(IParseContext context)
        {
            var expression = context.ReadNode<IExpression>();
            context.ReadOrPanic(TokenKind.SemiColon);
            return expression;
        }

        private static Declaration ReadDeclaration(IParseContext context)
        {
            var declaration = context.ReadNode<Declaration>();
            context.ReadOrPanic(TokenKind.SemiColon);
            return declaration;
        }

        private static IStatement ReadReturn(IParseContext context)
        {
            context.ReadOrPanic(TokenKind.Return);
            IExpression returnExpression = null;
            if (context.NextCodeToken.Kind != TokenKind.SemiColon)
                returnExpression = context.ReadNode<IExpression>();
            context.ReadOrPanic(TokenKind.SemiColon);
            return new Return(returnExpression);
        }

        private IStatement ReadBlock(IParseContext context)
        {
            context.ReadOrPanic(TokenKind.OpenBraces);
            var statements = new List<IStatement>();
            while (context.NextCodeToken.Kind != TokenKind.CloseBraces)
                statements.Add(Read(context));
            context.ReadOrPanic(TokenKind.CloseBraces);
            return new CodeBlock(statements.ToArray());
        }

        private IStatement ReadFor(IParseContext context)
        {
            context.ReadOrPanic(TokenKind.For);
            context.ReadOrPanic(TokenKind.OpenParanthesis);
            var variable = context.NextCodeToken.Kind != TokenKind.SemiColon ? context.ReadNode<Declaration>() : null;
            context.ReadOrPanic(TokenKind.SemiColon);
            var condition = context.NextCodeToken.Kind != TokenKind.SemiColon ? context.ReadNode<IExpression>() : null;
            context.ReadOrPanic(TokenKind.SemiColon);
            var increment = context.NextCodeToken.Kind != TokenKind.CloseParanthesis ? context.ReadNode<IExpression>() : null;
            context.ReadOrPanic(TokenKind.CloseParanthesis);
            return new For(Read(context), variable, condition, increment);
        }

        private IStatement ReadWhile(IParseContext context)
        {
            context.ReadOrPanic(TokenKind.While);
            context.ReadOrPanic(TokenKind.OpenParanthesis);
            var condition = context.ReadNode<IExpression>();
            context.ReadOrPanic(TokenKind.CloseParanthesis);
            return new While(condition, Read(context));
        }

        private IStatement ReadIf(IParseContext context)
        {
            context.ReadOrPanic(TokenKind.If);
            context.ReadOrPanic(TokenKind.OpenParanthesis);
            var condition = context.ReadNode<IExpression>();
            context.ReadOrPanic(TokenKind.CloseParanthesis);
            var instruction = Read(context);

            if (context.NextCodeToken.Kind == TokenKind.Else)
            {
                context.ReadCodeToken();
                return new If(condition, instruction, Read(context));
            }

            return new If(condition, instruction);
        }
    }
}