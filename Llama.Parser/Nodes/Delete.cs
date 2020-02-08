namespace Llama.Parser.Nodes
{
    using Lexer;

    public class Delete : IStatement
    {
        public Token Identifier { get; }

        public Delete(Token identifier)
        {
            Identifier = identifier;
        }
    }
}