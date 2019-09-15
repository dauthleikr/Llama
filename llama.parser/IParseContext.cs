namespace Llama.Parser
{
    using Lexer;

    public interface IParseContext
    {
        Token NextCodeToken { get; }

        Token ReadCodeToken();
        TNode ReadNode<TNode>();
        void Panic(string message);
    }
}