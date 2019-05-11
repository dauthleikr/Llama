using System;
using System.Collections.Generic;
using System.Text;

namespace llama.syntax.Tokens
{
    public class ExpressionParanthesisToken : ExpressionToken
    {
        public readonly OpenParanthesisToken OpenParanthesisToken;
        public readonly ExpressionToken SubExpression;
        public readonly CloseParanthesisToken CloseParanthesisToken;

        public ExpressionParanthesisToken(OpenParanthesisToken openParanthesis, ExpressionToken subExpression, CloseParanthesisToken closeParanthesis)
        {
            OpenParanthesisToken = openParanthesis;
            SubExpression = subExpression;
            CloseParanthesisToken = closeParanthesis;
        }

        public override void WriteSourceRecursive(ITextOutput textOutput, bool codeOnly = true)
        {
            OpenParanthesisToken.WriteSourceRecursive(textOutput, codeOnly);
            SubExpression.WriteSourceRecursive(textOutput, codeOnly);
            CloseParanthesisToken.WriteSourceRecursive(textOutput, codeOnly);
        }

        public static bool TryParse(ISourceReader reader, out ExpressionParanthesisToken result)
        {
            var start = reader.Position;
            if (!OpenParanthesisToken.TryParse(reader, out var openParanthesis))
            {
                result = null;
                return false;
            }

            if (!ExpressionToken.TryParse(reader, out var expr))
            {
                reader.Position = start;
                result = null;
                return false;
            }

            if (!CloseParanthesisToken.TryParse(reader, out var closeParathesis))
            {
                reader.Position = start;
                result = null;
                return false;
            }

            result = new ExpressionParanthesisToken(openParanthesis, expr, closeParathesis);
            return true;
        }

        public static bool MayStartWith(char character) => OpenParanthesisToken.MayStartWith(character);
    }
}
