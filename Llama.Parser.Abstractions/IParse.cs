namespace Llama.Parser.Abstractions
{
    public interface IParse<out T>
    {
        T Read(IParseContext context);
    }
}