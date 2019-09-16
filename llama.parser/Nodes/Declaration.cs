namespace Llama.Parser.Nodes
{
    using Lexer;

    internal class Declaration
    {
        public Type Type { get; }
        public Token Identifier { get; }
        public IExpression InitialValue { get; }

        public Declaration(Type type, Token identifier, IExpression initialValue = null)
        {
            Type = type;
            Identifier = identifier;
            InitialValue = initialValue;
        }
    }
}