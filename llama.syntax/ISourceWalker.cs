using System;
using System.Collections.Generic;
using System.Text;

namespace llama.syntax
{
    using Tokens;

    public interface ISourceWalker
    {
        void Walk(IToken token);
        void Walk(INonCodeToken token);
    }
}
