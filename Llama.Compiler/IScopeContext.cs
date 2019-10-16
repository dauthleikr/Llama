namespace Llama.Compiler
{
    using Parser.Nodes;

    public interface IScopeContext
    {
        int TotalStackSpace { get; }
        int GetCalleeParameterOffset(int index);
        int GetLocalOffset(string identifier);
        Type GetLocalType(string identifier);
        ExpressionResult GetLocalReference(string identifier);
        FunctionDeclaration GetFunctionDeclaration(string identifier);
        FunctionImport GetFunctionImport(string identifier);
        bool IsLocalDefined(string identifier);
        void DefineLocal(string identifier, Type type);
        void PushScope();
        void PopScope();
    }
}