namespace Llama.Parser.Nodes
{
    using Lexer;

    internal class FunctionDeclaration
    {
        public class Parameter
        {
            public Type ParameterType { get; }
            public Token ParameterIdentifier { get; }

            public Parameter(Type parameterType, Token parameterIdentifier)
            {
                ParameterType = parameterType;
                ParameterIdentifier = parameterIdentifier;
            }
        }

        public Type ReturnType { get; }
        public Token Identifier { get; }
        public Parameter[] Parameters { get; }

        public FunctionDeclaration(Type returnType, Token identifier, params Parameter[] parameters)
        {
            ReturnType = returnType;
            Identifier = identifier;
            Parameters = parameters;
        }
    }
}