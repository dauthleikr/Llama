using System;
using System.Collections.Generic;
using System.Text;

namespace llama.syntax.Tokens
{
    public class CommaToken : TokenBase
    {
        public override void WriteSourceRecursive(ITextOutput textOutput, bool codeOnly = true) => WriteSource(textOutput, ",", !codeOnly);

        public static bool TryParse(ISourceReader reader, out CommaToken result)
        {
            if (!MayStartWith(reader.Peek()))
            {
                result = null;
                return false;
            }

            reader.Eat(1);
            result = new CommaToken { PostNonCodeToken = NonCodeToken.ParseOrNull(reader) };
            return true;
        }

        public static bool MayStartWith(char character) => character == ',';
    }
}
