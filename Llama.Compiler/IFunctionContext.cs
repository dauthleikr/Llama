namespace Llama.Compiler
{
    internal interface IFunctionContext
    {
        int TotalFunctionStackSpace { get; }
        int GetLocalOffset(string identifier);
    }
}