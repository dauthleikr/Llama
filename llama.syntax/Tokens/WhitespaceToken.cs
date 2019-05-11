using System;
using System.Collections.Generic;
using System.Text;

namespace llama.syntax.Tokens
{
    public class WhitespaceToken : INonCodeToken
    {
        public readonly string Whitespaces;

        public WhitespaceToken(string whitespaces) => Whitespaces = whitespaces;

        public void WalkRecursive(ISourceWalker walker) => walker.Walk(this);

        public static WhitespaceToken ParseOrNull(ISourceReader reader)
        {
            var whitespaces = new StringBuilder();
            while (!reader.IsAtEnd)
            {
                var peekChar = reader.Peek();
                if (!char.IsWhiteSpace(peekChar) && !char.IsControl(peekChar))
                    break;
                whitespaces.Append(reader.ReadChar());
            }
            return whitespaces.Length == 0 ? null : new WhitespaceToken(whitespaces.ToString());
        }

        public override string ToString() => Whitespaces;
    }
}
