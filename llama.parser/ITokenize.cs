namespace Llama.Parser
{
    public interface ITokenize
    {
        bool TryRead(string src, ref int pos, out Token result);
    }
}