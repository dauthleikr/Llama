using System;
using System.Collections.Generic;
using System.Text;

namespace Llama.Parser.Tokens
{
    public class CommaToken : AtomicToken<CommaToken>
    {
        protected override string ToStringInternal() => ",";
    }
}
