namespace Llama.Compiler
{
    internal interface IFunctionContext
    {
        int GetLocalOffset(string identifier);
    }
}