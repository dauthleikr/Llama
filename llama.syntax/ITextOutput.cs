using System;
using System.Collections.Generic;
using System.Text;

namespace llama.syntax
{
    public interface ITextOutput
    {
        void Write(string text);
        void Write(StringBuilder text);
        void Write(ReadOnlySpan<char> text);
        void Write(char[] text);
    }
}
