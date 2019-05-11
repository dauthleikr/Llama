namespace Llama.Parser.Framework
{
    public interface ISourceWalker
    {
        void Walk(IToken token);
        void Walk(INonCode token);
    }
}