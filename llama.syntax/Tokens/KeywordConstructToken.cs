using System;
using System.Collections.Generic;
using System.Text;

namespace llama.syntax.Tokens
{
    public abstract class KeywordConstructToken : StatementToken
    {
        public static bool TryParse(ISourceReader reader, out KeywordConstructToken result)
        {
            throw new NotImplementedException();
        }

        public static bool MayStartWith(char character) => throw new NotImplementedException();
    }
}
