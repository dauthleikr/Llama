namespace Llama.Compiler
{
    using Parser.Nodes;

    public interface IScopeContext
    {
        int TotalFunctionStackSpace { get; }
        int GetLocalOffset(string identifier);
        Type GetLocalType(string identifier);
        bool HasLocal(string identifier);
    }
}