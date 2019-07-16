namespace Llama.Parser.Parsers
{
    using Framework;
    using Language;
    using Tokens;
    using Tokens.Expressions;

    public class ExpressionParser : ParserBase<IExpressionToken>
    {
        public override ITokenizationResult<IExpressionToken> TryReadToken(ISourceReader reader, IParseContext context, INonCodeParser nonCodeParser)
        {
            return TryReadInitial(reader, context)
                .ImproveableWith(initial => TryReadGreedyContinuation(reader, context, initial));
        }

        private ITokenizationResult<IExpressionToken> TryReadInitial(ISourceReader reader, IParseContext context)
        {
            if (context.TryReadToken<NumericLiteralToken>(out var numericLiteral))
                return numericLiteral;
            if (context.TryReadToken<IdentifierToken>(out var identifier))
                return identifier;
            return ErrorExpectedToken(reader);
        }

        private ITokenizationResult<IExpressionToken> TryReadGreedyContinuation(ISourceReader reader, IParseContext context, IExpressionToken initialToken)
        {
            if (context.TryReadToken<BinaryOperatorToken>(out var binaryOperator))
                return TryReadGreedyContinuationBinaryOperator(reader, context, initialToken, binaryOperator);
            if (context.TryReadToken<FunctionCallToken>(out var functionCall))
                return TryReadGreedyContinuation(reader, context, new FunctionCallFullToken(initialToken, functionCall));
            return initialToken is ITokenizationResult<IExpressionToken> result ? result : new TokenizationResult<IExpressionToken>(initialToken);
        }

        private static void a(TokenBase<IExpressionToken> wow) { }

        private static BinaryOperationToken GetPrecedenceDominatedToken(BinaryOperationToken root, byte precedence)
        {
            if (root.BinaryOperator.Precedence >= precedence)
                return null;

            if (root.Left is BinaryOperationToken binaryOperation)
                return GetPrecedenceDominatedToken(binaryOperation, precedence) ?? root;
            return root;
        }

        private ITokenizationResult<IExpressionToken> TryReadGreedyContinuationBinaryOperator(ISourceReader reader, IParseContext context, IExpressionToken leftToken, BinaryOperatorToken operatorToken)
        {
            var positionExpectedExpression = reader.Position;
            if (!context.TryReadToken<IExpressionToken>(out var rightToken))
                return ErrorExpectedToken(reader, 1);

            BinaryOperationToken binaryOperation = null;
            if (rightToken is BinaryOperationToken rightBinaryOperation)
            {
                var precedenceDominated = GetPrecedenceDominatedToken(rightBinaryOperation, operatorToken.Precedence);
                if (precedenceDominated != null)
                {
                    var newTornOperation = new BinaryOperationToken(leftToken, operatorToken, precedenceDominated.Left);
                    precedenceDominated.Left = newTornOperation;
                    binaryOperation = rightBinaryOperation;
                }
            }

            var result = binaryOperation ?? new BinaryOperationToken(leftToken, operatorToken, rightToken);
            if (result.BinaryOperator.Operator == BinaryOperator.MemberAccess && !(result.Right is IdentifierToken))
                return Error(positionExpectedExpression, $"Expected identifier on the right side of {nameof(BinaryOperator.MemberAccess)}-operator", 2, (int) (reader.Position - positionExpectedExpression));
            return result;
        }

        public override bool IsPlausible(ISourcePeeker reader, IParseContext context) => !reader.IsAtEnd; // todo
    }
}