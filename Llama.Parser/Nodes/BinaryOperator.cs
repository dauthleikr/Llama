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
                switch (Operator.Kind)
                {
                    case TokenKind.Plus:
                        return 96;
                    case TokenKind.Minus:
                        return 96;
                    case TokenKind.Equals:
                        return 90;
                    case TokenKind.NotEquals:
                        return 90;
                    case TokenKind.OpenAngularBracket:
                        return 92;
                    case TokenKind.CloseAngularBracket:
                        return 92;
                    case TokenKind.GreaterEquals:
                        return 92;
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