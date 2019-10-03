namespace Llama.Compiler
{
    using System;

    internal class BadLiteralException : Exception
    {
        public BadLiteralException(string literal) : base($"Bad literal: {literal}") { }
    }
}