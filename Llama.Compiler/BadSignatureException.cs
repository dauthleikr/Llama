namespace Llama.Compiler
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using Parser.Nodes;
    using Type = Parser.Nodes.Type;

    public class BadSignatureException : Exception
    {
        public BadSignatureException(FunctionDeclaration expected, IEnumerable<Type> actual) : base(
            $"{expected.Identifier.RawText}: Expected signature {GetParameterSignatureString(expected)} but got ({string.Join(", ", actual.Select(item => item.ToString()))})"
        ) { }

        public BadSignatureException(FunctionDeclaration expected) : base(
            $"{expected.Identifier.RawText}: Expected signature {GetParameterSignatureString(expected)}"
        ) { }

        private static string GetParameterSignatureString(FunctionDeclaration decl) =>
            $"({string.Join(", ", decl.Parameters.Select(par => par.ParameterType.ToString()))})";
    }
}