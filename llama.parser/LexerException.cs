namespace Llama.Parser
{
    using System;

    internal class LexerException : Exception
    {
        public LexerException(string msg) : base(msg) { }
    }
}