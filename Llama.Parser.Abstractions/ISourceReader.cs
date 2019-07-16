namespace Llama.Parser.Framework
{
    public interface ISourceReader : ISourcePeeker
    {
        new long Position { get; set; }

        string Read(int characters);

        bool ReadIfEqual(char character);

        char ReadChar();

        void Eat(int amount = 1);
    }
}