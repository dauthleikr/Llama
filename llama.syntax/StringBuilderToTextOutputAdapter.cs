using System;
using System.Collections.Generic;
using System.Text;

namespace llama.syntax
{
    public class StringBuilderToTextOutputAdapter : ITextOutput
    {
        private readonly StringBuilder _stringBuilder = new StringBuilder();

        public void Write(string text) => _stringBuilder.Append(text);
        public void Write(StringBuilder text) => _stringBuilder.Append(text);
        public void Write(ReadOnlySpan<char> text) => _stringBuilder.Append(text);
        public void Write(char[] text) => _stringBuilder.Append(text);
        public override string ToString() => _stringBuilder.ToString();
    }
}
