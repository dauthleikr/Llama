namespace Llama.Compiler
{
    using Parser.Nodes;

    public interface IScopeContext
    {
        int TotalStackSpace { get; }
        int GetLocalOffset(string identifier);
        Type GetLocalType(string identifier);
        bool IsLocalDefined(string identifier);
        void DefineLocal(string identifier, Type type);
        void PushScope();
        void PopScope();
    }
}