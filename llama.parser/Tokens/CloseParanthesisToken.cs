using System;
using System.Collections.Generic;
using System.Text;

namespace Llama.Parser.Tokens
{
    public class CloseParanthesisToken : AtomicToken<CloseParanthesisToken>
    {
        protected override string ToStringInternal() => ")";
    }
}
