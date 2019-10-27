namespace Llama.Parser.Lexer
{
    public interface ITokenize
    {
        bool TryRead(string src, int pos, out Token result);
    }
}