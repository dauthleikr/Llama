namespace Llama.Parser
{
    using System;

    internal class ParserException : Exception
    {
        public ParserException(string msg) : base(msg) { }
    }
}