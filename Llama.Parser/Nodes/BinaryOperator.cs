namespace Llama.Parser.Nodes
{
    using System;
    using Lexer;

    public class BinaryOperator
    {
        public Token Operator { get; }

        public int Precedence
        {
            get
            {
                // https://github.com/dauthleikr/llama/blob/4e8bbbfe45709f83d898b259fb5c79bc57bb0b8a/llama.parser/Language/BinaryOperators.cs
                switch (Operator.Kind)
                {
                    case TokenKind.Modolu:
                    case TokenKind.Divide:
                    case TokenKind.Pointer:
                        return 98;
                    case TokenKind.Plus:
                    case TokenKind.Minus:
                        return 96;
                    case TokenKind.Equals:
                    case TokenKind.NotEquals:
                        return 90;
                    case TokenKind.OpenAngularBracket:
                    case TokenKind.CloseAngularBracket:
                    case TokenKind.GreaterEquals:
                    case TokenKind.SmallerEquals:
                        return 92;
                    case TokenKind.Assignment:
                        return 49;
                    default:
                        throw new ArgumentOutOfRangeException();
                }
            }
        }

        public BinaryOperator(Token @operator) => Operator = @operator;

        public static bool IsTokenKindValid(TokenKind kind) =>
            kind == TokenKind.Plus ||
            kind == TokenKind.Minus ||
            kind == TokenKind.Divide ||
            kind == TokenKind.Pointer ||
            kind == TokenKind.Modolu ||
            kind == TokenKind.Assignment ||
            kind == TokenKind.Equals ||
            kind == TokenKind.NotEquals ||
            kind == TokenKind.OpenAngularBracket ||
            kind == TokenKind.CloseAngularBracket ||
            kind == TokenKind.GreaterEquals ||
            kind == TokenKind.SmallerEquals;
    }
}