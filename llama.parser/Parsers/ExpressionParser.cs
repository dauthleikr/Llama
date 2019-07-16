namespace Llama.Parser.Parsers
{
    using Abstractions;
    using Entities;
    using Entities.Expressions;
    using Framework;
    using Language;

    public class ExpressionParser : ParserBase<IExpressionEntity>
    {
        public override IParseResult<IExpressionEntity> TryRead(ISourceReader reader, IParseContext context, INonCodeParser nonCodeParser)
        {
            return TryReadInitial(reader, context)
                .ImproveableWith(initial => TryReadGreedyContinuation(reader, context, initial));
        }

        private IParseResult<IExpressionEntity> TryReadInitial(ISourceReader reader, IParseContext context)
        {
            if (context.TryRead<NumericLiteralEntity>(out var numericLiteral))
                return numericLiteral;
            if (context.TryRead<IdentifierEntity>(out var identifier))
                return identifier;
            return ErrorExpectedEntity(reader);
        }

        private IParseResult<IExpressionEntity> TryReadGreedyContinuation(ISourceReader reader, IParseContext context, IExpressionEntity initialEntity)
        {
            if (context.TryRead<BinaryOperatorEntity>(out var binaryOperator))
                return TryReadGreedyContinuationBinaryOperator(reader, context, initialEntity, binaryOperator);
            if (context.TryRead<FunctionCallEntity>(out var functionCall))
                return TryReadGreedyContinuation(reader, context, new FunctionCallFullEntity(initialEntity, functionCall));
            return initialEntity is IParseResult<IExpressionEntity> result ? result : new ParseResult<IExpressionEntity>(initialEntity);
        }

        private static void a(EntityBase<IExpressionEntity> wow) { }

        private static BinaryOperationEntity GetPrecedenceDominatedEntity(BinaryOperationEntity root, byte precedence)
        {
            if (root.BinaryOperator.Precedence >= precedence)
                return null;

            if (root.Left is BinaryOperationEntity binaryOperation)
                return GetPrecedenceDominatedEntity(binaryOperation, precedence) ?? root;
            return root;
        }

        private IParseResult<IExpressionEntity> TryReadGreedyContinuationBinaryOperator(ISourceReader reader, IParseContext context, IExpressionEntity leftEntity, BinaryOperatorEntity operatorEntity)
        {
            var positionExpectedExpression = reader.Position;
            if (!context.TryRead<IExpressionEntity>(out var rightToken))
                return ErrorExpectedEntity(reader, 1);

            BinaryOperationEntity binaryOperation = null;
            if (rightToken is BinaryOperationEntity rightBinaryOperation)
            {
                var precedenceDominated = GetPrecedenceDominatedEntity(rightBinaryOperation, operatorEntity.Precedence);
                if (precedenceDominated != null)
                {
                    var newTornOperation = new BinaryOperationEntity(leftEntity, operatorEntity, precedenceDominated.Left);
                    precedenceDominated.Left = newTornOperation;
                    binaryOperation = rightBinaryOperation;
                }
            }

            var result = binaryOperation ?? new BinaryOperationEntity(leftEntity, operatorEntity, rightToken);
            if (result.BinaryOperator.Operator == BinaryOperator.MemberAccess && !(result.Right is IdentifierEntity))
                return Error(positionExpectedExpression, $"Expected identifier on the right side of {nameof(BinaryOperator.MemberAccess)}-operator", 2, (int) (reader.Position - positionExpectedExpression));
            return result;
        }

        public override bool IsPlausible(ISourcePeeker reader, IParseContext context) => !reader.IsAtEnd; // todo
    }
}