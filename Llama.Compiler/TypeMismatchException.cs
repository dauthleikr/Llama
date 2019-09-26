namespace Llama.Compiler
{
    using System;

    internal class TypeMismatchException : Exception
    {
        public TypeMismatchException(string expected, string actual) : base($"Type mismatch: Expected {expected} but got {actual}") { }
    }
}