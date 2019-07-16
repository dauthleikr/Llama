namespace Llama.Parser.Abstractions
{
    public interface ISourceWalker
    {
        void Walk(IEntity entity);
        void Walk(INonCode token);
    }
}