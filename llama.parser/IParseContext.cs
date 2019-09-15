namespace Llama.Parser
{
    using System;

    public interface IParseContext<out T> where T : Enum
    {
        IToken<T> NextCodeToken { get; }

        IToken<T> ReadCodeToken();
        TNode ReadNode<TNode>();
        void Panic(string message);
    }
}