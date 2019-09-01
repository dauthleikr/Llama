namespace Llama.PE.Builder
{
    using System;

    internal class BadSectionNameException : Exception
    {
        public BadSectionNameException(string badName) : base($"The name \"{badName}\" is not a valid section name (up to 8 ASCII chars)") { }
    }
}