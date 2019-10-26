namespace Llama.Parser.Lexer
{
    public interface ISkipTrivia
    {
        int GetPositionAfterTrivia(string src, int pos);
    }
}