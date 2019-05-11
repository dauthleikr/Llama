using System;
using System.Collections.Generic;
using System.Text;

namespace llama.syntax.Tokens
{
    using System.ComponentModel.Design;

    public abstract class ExpressionToken : StatementToken
    {
        public static bool TryParse(ISourceReader reader, out ExpressionToken result)
        {
            if (reader.IsAtEnd)
            {
                result = null;
                return false;
            }

            var peekChar = reader.Peek();
            var startPos = reader.Position;
            ExpressionToken resultToken = null;

            if (ExpressionIntegerLiteralToken.MayStartWith(peekChar) && ExpressionIntegerLiteralToken.TryParse(reader, out var integerLiteral))
                resultToken = integerLiteral;
            else if (ExpressionIdentifierToken.MayStartWith(peekChar) && ExpressionIdentifierToken.TryParse(reader, out var identifier))
                resultToken = identifier;
            else if (ExpressionParanthesisToken.MayStartWith(peekChar) && ExpressionParanthesisToken.TryParse(reader, out var paranthesisExpression))
                resultToken = paranthesisExpression;

            if (resultToken == null)
            {
                if (ExpressionIntegerLiteralToken.MayStartWith(peekChar))
                    throw new TokenizerException(startPos, "Bad literal in expression");
                if (ExpressionIdentifierToken.MayStartWith(peekChar))
                    throw new TokenizerException(startPos, "Bad identifier in expression");
                if (ExpressionParanthesisToken.MayStartWith(peekChar))
                    throw new TokenizerException(startPos, "Paranthesis '(' with no valid expression or ')' found");

                result = null;
                return false;
            }

            try
            {
                result = TryParseGreedyContinuation(reader, resultToken);
            }
            catch (TokenizerException)
            {
                reader.Position = startPos;
                throw;
            }
            return true;
        }

        private static ExpressionToken TryParseGreedyContinuationBinaryOperator(ISourceReader reader, ExpressionToken leftExpression, BinaryOperatorToken operatorToken)
        {
            ExpressionBinaryOperationToken GetPrecedenceDominatedToken(ExpressionBinaryOperationToken root, byte precedence)
            {
                if (root.BinaryOperator.Precedence > precedence)
                    return null;

                if (root.Left is ExpressionBinaryOperationToken binOperation)
                    return GetPrecedenceDominatedToken(binOperation, precedence) ?? root;
                return root;
            }

            var positionExpectedExpression = reader.Position;
            if (!TryParse(reader, out var rightExpression))
            {
                throw new TokenizerExceptionWithLength(positionExpectedExpression,
                    (int)Math.Max(reader.Position - positionExpectedExpression, 1),
                    $"Expected expression on the right-hand side of binary operation '{BinaryOperatorToken.EnumToOperator[operatorToken.Operator]}'");
            }

            ExpressionBinaryOperationToken binaryOperation = null;
            if (rightExpression is ExpressionBinaryOperationToken rightBinaryOperation)
            {
                var precedenceDominated = GetPrecedenceDominatedToken(rightBinaryOperation, operatorToken.Precedence);
                if (precedenceDominated != null)
                {
                    var newTornOperation = new ExpressionBinaryOperationToken(leftExpression, operatorToken, precedenceDominated.Left);
                    precedenceDominated.Left = newTornOperation;
                    binaryOperation = rightBinaryOperation;
                }
            }

            var result = binaryOperation ?? new ExpressionBinaryOperationToken(leftExpression, operatorToken, rightExpression);
            if (result.BinaryOperator.Operator == BinaryOperator.MemberAccess && !(result.Right is ExpressionIdentifierToken))
            {
                throw new TokenizerExceptionWithLength(positionExpectedExpression,
                    (int)Math.Max(reader.Position - positionExpectedExpression, 1),
                    $"Expected identifier on the right side of {nameof(BinaryOperator.MemberAccess)}-operator");
            }

            return result;
        }

        private static ExpressionToken TryParseGreedyContinuationFunctionCall(ISourceReader reader, ExpressionToken previousToken)
        {
            if (!(previousToken is ExpressionIdentifierToken identifierToken)) // function call
                throw new TokenizerException(reader.Position, "Unexpected '(' (missing operator or bad function call?)");
            return ExpressionFunctionCallToken.TryParseAfterIdentifier(reader, identifierToken, out var functionCallToken) ? functionCallToken : previousToken;
        }

        private static ExpressionToken TryParseGreedyContinuation(ISourceReader reader, ExpressionToken previousToken)
        {
            var peekChar = reader.Peek();
            if (ExpressionFunctionCallToken.MayStartWithAfterIdentifier(peekChar))
                return TryParseGreedyContinuation(reader, TryParseGreedyContinuationFunctionCall(reader, previousToken));
            if (BinaryOperatorToken.MayStartWith(peekChar) && BinaryOperatorToken.TryParse(reader, out var operatorToken))
                return TryParseGreedyContinuationBinaryOperator(reader, previousToken, operatorToken);

            return previousToken;
        }
    }
}
