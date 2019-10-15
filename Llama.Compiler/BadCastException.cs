namespace Llama.Compiler
{
    using System;

    internal class BadCastException : Exception
    {
        public BadCastException(string message) : base(message) { }
    }
}