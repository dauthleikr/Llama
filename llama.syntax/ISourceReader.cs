using System;
using System.Collections.Generic;
using System.Text;

namespace llama.syntax
{
    using System.Dynamic;

    public interface ISourceReader
    {
        bool IsAtEnd { get; }
        long Position { get; set; }

        string ReadIdentifier();

        char Peek();
        char PeekFurther(int offsetToPeek);

        bool ReadFurtherIfEqual(int offset, char character);

        char ReadChar();

        void Eat(int amount);

        ReadOnlySpan<char> PeekNext(int count);

        void ReportSyntaxError(long position, int length, string error);
        void ReportSyntaxError(string error);
    }
}
