namespace Llama.Compiler
{
    using Parser.Nodes;

    public interface ISymbolResolver
    {
        int TotalStackSpace { get; }
        string CurrentFunctionIdentifier { get; }
        int GetCalleeParameterOffset(int index);
        int GetLocalOffset(string identifier);
        Type GetLocalType(string identifier);
        ExpressionResult GetLocalReference(string identifier);
        FunctionDeclaration GetFunctionDeclaration(string identifier);
        FunctionImport GetFunctionImport(string identifier);
        bool IsLocalDefined(string identifier);
        void DefineLocal(string identifier, Type type);
        void DefineFunctionLocal(string identifier, Type type);
        void PushLocalScope();
        void PopLocalScope();
    }
}