namespace Llama.Parser.Lexer
{
    public interface ITokenize
    {
        bool TryRead(string src, ref int pos, out Token result);
    }
}