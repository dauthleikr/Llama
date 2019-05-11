using System;
using System.Collections.Generic;
using System.Text;

namespace llama.syntax.Tokens
{
    class ReturnToken : TokenBase
    {
        private const string Text = "return";

        public override void WriteSourceRecursive(ITextOutput textOutput, bool codeOnly = true) => WriteSource(textOutput, Text, !codeOnly);

        public static bool TryParse(ISourceReader reader, out ReturnToken token)
        {
            var success = reader.TryRead(Text);
            token = success ? new ReturnToken { PostNonCodeToken = NonCodeToken.ParseOrNull(reader) } : null;
            return success;
        }
    }
}
