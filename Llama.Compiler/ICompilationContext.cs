namespace Llama.Compiler
{
    using Parser.Nodes;
    using spit;

    public interface ICompilationContext
    {
        IAddressFixer AddressLinker { get; }

        ExpressionResult CompileExpression<T>(
            T expression,
            CodeGen codeGen,
            StorageManager storageManager,
            PreferredRegister target,
            IScopeContext scope
        ) where T : IExpression;

        void CompileStatement<T>(T statement, CodeGen codeGen, StorageManager storageManager, IScopeContext scope) where T : IStatement;
    }
}