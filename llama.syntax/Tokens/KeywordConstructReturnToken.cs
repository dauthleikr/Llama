using System;
using System.Collections.Generic;
using System.Text;

namespace llama.syntax.Tokens
{
    class KeywordConstructReturnToken : KeywordConstructToken
    {
        public readonly ReturnToken Keyword;
        public readonly ExpressionToken Expression;

        public KeywordConstructReturnToken(ReturnToken keyword, ExpressionToken expression)
        {
            Keyword = keyword;
            Expression = expression;
        }

        public override void WriteSourceRecursive(ITextOutput textOutput, bool codeOnly = true)
        {
            Keyword.WriteSourceRecursive(textOutput, codeOnly);
            Expression.WriteSourceRecursive(textOutput, codeOnly);
        }

        public override void WalkRecursive(ISourceWalker walker, bool codeChildrenOnly = true)
        {
            Expression.WalkRecursive(walker, codeChildrenOnly);
            Keyword.WalkRecursive(walker, codeChildrenOnly);
            base.WalkRecursive(walker, codeChildrenOnly);
        }

        public static bool TryParse(ISourceReader reader, out KeywordConstructReturnToken keywordConstruct)
        {
            var start = reader.Position;
            if (!ReturnToken.TryParse(reader, out var keyword) || !ExpressionToken.TryParse(reader, out var expression))
            {
                reader.Position = start;
                keywordConstruct = null;
                return false;
            }
            keywordConstruct = new KeywordConstructReturnToken(keyword, expression);
            return true;
        }

        public static bool MayStartWith(char character) => ExpressionIdentifierToken.MayStartWith(character);
    }
}
