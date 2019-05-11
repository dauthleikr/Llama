using System;
using System.Collections.Generic;
using System.Text;

namespace llama.syntax.Tokens
{
    public class ExpressionIdentifierToken : ExpressionToken
    {
        public readonly string Identifier;
        public ExpressionIdentifierToken(string identifier) => Identifier = identifier;

        public override void WriteSourceRecursive(ITextOutput textOutput, bool codeOnly = true) => WriteSource(textOutput, Identifier, !codeOnly);

        public static bool TryParse(ISourceReader reader, out ExpressionIdentifierToken result)
        {
            var name = reader.ReadIdentifier();
            if (string.IsNullOrEmpty(name))
            {
                result = null;
                return false;
            }

            result = new ExpressionIdentifierToken(name) { PostNonCodeToken = NonCodeToken.ParseOrNull(reader) };
            return true;
        }

        public static bool MayStartWith(char character) => character == '_' || char.IsLetter(character);
    }
}
