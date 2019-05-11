using System;
using System.Collections.Generic;
using System.Text;

namespace llama.syntax.Tokens
{
    using System.ComponentModel.Design;
    using System.Linq;

    public class NonCodeToken : INonCodeToken
    {
        public readonly List<INonCodeToken> NonCode;
        public NonCodeToken(IEnumerable<INonCodeToken> nonCodeTokens) => NonCode = nonCodeTokens.ToList();

        public void WalkRecursive(ISourceWalker walker)
        {
            walker.Walk(this);
            foreach (var token in NonCode)
                token.WalkRecursive(walker);
        }

        public override string ToString()
        {
            var stringBuilder = new StringBuilder();
            foreach (var nonCodeToken in NonCode)
                stringBuilder.Append(nonCodeToken);
            return stringBuilder.ToString();
        }

        public static NonCodeToken ParseOrNull(ISourceReader reader)
        {
            var tokens = new List<INonCodeToken>();
            while (!reader.IsAtEnd)
            {
                var peekChar = reader.Peek();
                if (char.IsWhiteSpace(peekChar))
                {
                    var whitespaceToken = WhitespaceToken.ParseOrNull(reader);
                    if (whitespaceToken != null)
                        tokens.Add(whitespaceToken);
                }
                else if (peekChar == '/')
                {
                    var nextChar = reader.PeekFurther(1);
                    if (nextChar == '/' && LineCommentToken.TryParse(reader, out var lineCommentToken))
                        tokens.Add(lineCommentToken);
                    else if (nextChar == '*' && BlockCommentToken.TryParse(reader, out var blockCommentToken))
                        tokens.Add(blockCommentToken);
                    else
                        break;
                }
                else
                    break; // next char is neither whitespace nor any kind of comment
            }

            return tokens.Count == 0 ? null : new NonCodeToken(tokens);
        }
    }
}