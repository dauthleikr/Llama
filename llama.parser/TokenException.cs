namespace Llama.Parser
{
    using System;

    internal class TokenException : Exception
    {
        public TokenException(string msg) : base(msg) { }
    }
}