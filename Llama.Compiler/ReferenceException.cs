namespace Llama.Compiler
{
    using System;

    internal class ReferenceException : Exception
    {
        public ReferenceException(string msg) : base(msg) { }
    }
}