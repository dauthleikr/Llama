using System;
using System.Collections.Generic;
using System.Text;

namespace llama.syntax.Tokens
{
    public abstract class TokenBase : IToken
    {
        public NonCodeToken PreNonCodeToken { get; internal set; }
        public NonCodeToken PostNonCodeToken { get; internal set; }

        public virtual void WalkRecursive(ISourceWalker walker, bool codeChildrenOnly = true)
        {
            if (!codeChildrenOnly)
                PreNonCodeToken?.WalkRecursive(walker);
            walker.Walk(this);
            if (!codeChildrenOnly)
                PostNonCodeToken?.WalkRecursive(walker);
        }

        public abstract void WriteSourceRecursive(ITextOutput textOutput, bool codeOnly = true);

        protected void WriteSource(ITextOutput textOutput, string source, bool includeNonCode)
        {
            if (includeNonCode)
                textOutput.Write(PreNonCodeToken?.ToString());
            textOutput.Write(source);
            if (includeNonCode)
                textOutput.Write(PostNonCodeToken?.ToString());
        }

        public override string ToString()
        {
            var stringBuilderAdapter = new StringBuilderToTextOutputAdapter();
            WriteSourceRecursive(stringBuilderAdapter, false);
            return stringBuilderAdapter.ToString();
        }
    }
}
