namespace Llama.Parser.Lexer
{
    using System;

    internal class LexerException : Exception
    {
        public LexerException(string msg) : base(msg) { }
    }
}