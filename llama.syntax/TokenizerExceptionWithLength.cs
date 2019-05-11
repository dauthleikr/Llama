using System;
using System.Collections.Generic;
using System.Text;

namespace llama.syntax
{
    public class TokenizerExceptionWithLength : TokenizerException
    {
        public readonly int Length;

        public TokenizerExceptionWithLength(long position, int length, string error) : base(position, error)
        {
            Length = length;
        }
    }
}
