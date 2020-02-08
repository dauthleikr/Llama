namespace Llama.Compiler
{
    using Parser.Nodes;
    using spit;

    public interface ICompilationContext
    {
        ILinkingInfo AddressLinker { get; }

        ICompilationContext CreateChildContext();

        ExpressionResult CompileExpression<T>(
            T expression,
            CodeGen codeGen,
            StorageManager storageManager,
            PreferredRegister target,
            ISymbolResolver scope
        ) where T : IExpression;

        void CompileStatement<T>(T statement, CodeGen codeGen, StorageManager storageManager, ISymbolResolver scope) where T : IStatement;
    }
}