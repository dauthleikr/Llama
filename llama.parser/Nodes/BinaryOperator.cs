namespace Llama.Parser.Nodes
{
    using System;
    using Lexer;

    internal class BinaryOperator
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
                    case TokenKind.Assignment:
                        return 49;
                    default:
                        throw new ArgumentOutOfRangeException();
                }
            }
        }

        public BinaryOperator(Token @operator) => Operator = @operator;

        public static bool IsTokenKindValid(TokenKind kind) => kind == TokenKind.Plus || kind == TokenKind.Minus || kind == TokenKind.Assignment;
    }
}