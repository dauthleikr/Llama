using System;
using System.Collections.Generic;
using System.Text;

namespace llama.syntax
{
    public class TokenizerException : Exception
    {
        public readonly long Position;
        public readonly string Error;

        public TokenizerException(long position, string error) : base($"Unable to tokenize at position {position}: {error}")
        {
            Position = position;
            Error = error;
        }
    }
}
