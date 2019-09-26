namespace Llama.Compiler
{
    public interface IFunctionContext
    {
        int TotalFunctionStackSpace { get; }
        int GetLocalOffset(string identifier);
    }
}