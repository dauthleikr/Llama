namespace Llama.Compiler
{
    public interface IFactory<out T>
    {
        T Create();
    }
}