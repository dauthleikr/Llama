namespace Llama.Parser.Tests.DebugImplementations
{
    using System;
    using Abstractions;

    internal class StringSourceReader : ISourceReader
    {
        public bool IsAtEnd => Position >= _source.Length;
        public long Position { get; set; }
        private readonly char[] _source;

        public StringSourceReader(string source) => _source = source.ToCharArray();

        public string Read(int characters) => new string(_source, (int) Position, characters);

        public bool ReadIfEqual(char character)
        {
            if (Peek() != character)
                return false;
            Eat();
            return true;
        }

        public char ReadChar() => IsAtEnd ? '\0' : _source[Position++];

        public void Eat(int amount = 1)
        {
            Position += amount;
        }

        public char Peek() => IsAtEnd ? '\0' : _source[Position];

        public char PeekFurther(int offsetToPeek) => Position + offsetToPeek >= _source.Length || Position + offsetToPeek < 0 ? '\0' : _source[Position + offsetToPeek];

        public ReadOnlySpan<char> PeekNext(int count) => throw new NotImplementedException();
    }
}