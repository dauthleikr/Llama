namespace Llama.Linker
{
    using System;

    public class LinkException : Exception
    {
        public LinkException(string message) : base(message) { }
    }
}