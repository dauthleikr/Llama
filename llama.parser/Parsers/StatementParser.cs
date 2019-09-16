namespace Llama.Parser.Parsers
{
    using System.Collections.Generic;
    using Lexer;
    using Nodes;

    internal class StatementParser : IParse<IStatement>
    {
        public IStatement Read(IParseContext context)
        {
            switch (context.NextCodeToken.Kind)
            {
                case TokenKind.If:
                    return ReadIf(context);
                case TokenKind.While:
                    return ReadWhile(context);
                case TokenKind.For:
                    return ReadFor(context);
                case TokenKind.PrimitiveType:
                {
                    var declaration = context.ReadNode<Declaration>();
                    context.ReadOrPanic(TokenKind.SemiColon);
                    return declaration;
                }
                case TokenKind.OpenBraces:
                    return ReadBlock(context);
                default:
                {
                    var expression = context.ReadNode<IExpression>();
                    context.ReadOrPanic(TokenKind.SemiColon);
                    return expression;
                }
            }
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
            context.ReadOrPanic(TokenKind.While);
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