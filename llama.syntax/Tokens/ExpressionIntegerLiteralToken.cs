using System;
using System.Collections.Generic;
using System.Text;

namespace llama.syntax.Tokens
{
    using System.Numerics;

    public class ExpressionIntegerLiteralToken : ExpressionToken
    {
        public readonly string IntegerLiteral;
        public ExpressionIntegerLiteralToken(string integerLiteral) => IntegerLiteral = integerLiteral;

        public override void WriteSourceRecursive(ITextOutput textOutput, bool codeOnly = true) => WriteSource(textOutput, IntegerLiteral, !codeOnly);

        public static bool TryParse(ISourceReader reader, out ExpressionIntegerLiteralToken result)
        {
            var literal = new StringBuilder();
            var atLeastOneDigit = false;
            while (!reader.IsAtEnd)
            {
                var peekChar = reader.Peek();
                if (char.IsDigit(peekChar))
                {
                    atLeastOneDigit = true;
                    literal.Append(reader.ReadChar());
                }
                else if (atLeastOneDigit && peekChar == '_')
                    literal.Append(reader.ReadChar());
                else
                    break;
            }
            result = atLeastOneDigit ? new ExpressionIntegerLiteralToken(literal.ToString()) { PostNonCodeToken = NonCodeToken.ParseOrNull(reader) } : null;
            return atLeastOneDigit;
        }

        public static bool MayStartWith(char character) => char.IsDigit(character);
    }
}
