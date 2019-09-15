namespace Llama.Parser.Parsers
{
    using Lexer;
    using Nodes;

    internal class ExpressionParser : IParse<IExpression>
    {
        public IExpression Read(IParseContext context)
        {

        }

        private IExpression ReadUncontinuedExpression(IParseContext context)
        {
            if (context.NextCodeToken.Kind == TokenKind.OpenAngularBracket)
                return ReadTypeCastExpression(context);
            if (context.NextCodeToken.Kind == TokenKind.OpenParanthesis)
                return ReadParanthesisExpression(context);
            if (context.NextCodeToken.Kind == TokenKind.New)
                return ReadArrayAllocationExpression(context);
            if (UnaryOperator.IsTokenKindValid(context.NextCodeToken.Kind))
                return ReadUnaryOperatorExpression(context);
            return ReadAtomicExpression(context);
        }

        private IExpression TryReadContinuedExpression(IParseContext context, IExpression currentExpression)
        {

            if(BinaryOperator.IsTokenKindValid(context.NextCodeToken.Kind))
        }

        private IExpression ReadAtomicExpression(IParseContext context)
        {
            if (AtomicExpression.IsTokenKindValid(context.NextCodeToken.Kind))
                return new AtomicExpression(context.ReadCodeToken());

            context.Panic($"Expected {nameof(AtomicExpression)}");
            return null;
        }

        private IExpression ReadArrayAllocationExpression(IParseContext context)
        {
            context.ReadOrPanic(TokenKind.New);
            var allocationType = context.ReadNode<TypeNode>();
            context.ReadOrPanic(TokenKind.OpenSquareBracket);
            var count = Read(context);
            context.ReadOrPanic(TokenKind.CloseSquareBracket);
            return new ArrayAllocationExpression(allocationType, count);
        }

        private IExpression ReadUnaryOperatorExpression(IParseContext context)
        {
            var op = context.ReadNode<UnaryOperator>();
            return new UnaryOperatorExpression(op, ReadUncontinuedExpression(context));
        }

        private IExpression ReadTypeCastExpression(IParseContext context)
        {
            context.ReadOrPanic(TokenKind.OpenAngularBracket);
            var type = context.ReadNode<TypeNode>();
            context.ReadOrPanic(TokenKind.CloseAngularBracket);
            return new TypeCast(type, ReadUncontinuedExpression(context));
        }

        private IExpression ReadParanthesisExpression(IParseContext context)
        {
            context.ReadOrPanic(TokenKind.OpenParanthesis);
            var expression = Read(context);
            context.ReadOrPanic(TokenKind.CloseParanthesis);
            return expression;
        }


    }
}