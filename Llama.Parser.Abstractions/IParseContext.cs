namespace Llama.Parser.Abstractions
{
    using System;

    public interface IParseContext<out T> where T : Enum
    {
        IToken<T> NextToken { get; }

        IToken<T> ReadToken();
        TNode ReadNode<TNode>();
        void Panic();
    }
}