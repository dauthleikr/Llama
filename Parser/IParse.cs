namespace Llama.Parser
{
    public interface IParse<out T>
    {
        T Read(IParseContext context);
    }
}