namespace Llama.Parser.Abstractions
{
    using System;

    public interface ISourcePeeker
    {
        bool IsAtEnd { get; }
        long Position { get; }
        char Peek();
        char PeekFurther(int offsetToPeek);
        ReadOnlySpan<char> PeekNext(int count);
    }
}