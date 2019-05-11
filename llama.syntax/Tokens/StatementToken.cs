using System;
using System.Collections.Generic;
using System.Text;

namespace llama.syntax.Tokens
{
    public abstract class StatementToken : TokenBase
    {
        public static bool TryParse(ISourceReader reader, out StatementToken result)
        {
            result = null;
            if (KeywordConstructToken.MayStartWith(reader.Peek()) && KeywordConstructToken.TryParse(reader, out var keywordConstruct))
                result = keywordConstruct;
            else if (VariableDeclarationToken.TryParseWithOptionalAssignment(reader, out var varDeclaration))
                result = varDeclaration;


                return result == null;
        }
    }
}
