namespace Llama.Compiler
{
    using System;

    public class UnknownIdentifierException : Exception
    {
        public UnknownIdentifierException(string message) : base(message) { }
    }
}