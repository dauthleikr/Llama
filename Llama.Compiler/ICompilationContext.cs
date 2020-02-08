namespace Llama.Compiler
{
    using Parser.Nodes;
    using spit;

    public interface ICompilationContext
    {
        ILinkingInfo Linking { get; }
        ISymbolResolver Symbols { get; }
        StorageManager Storage { get; }
        CodeGen Generator { get; }

        ICompilationContext CreateChildContext();

        void CopyToContext(ICompilationContext other);

        ExpressionResult CompileExpression<T>(
            T expression,
            PreferredRegister target
        ) where T : IExpression;

        void CompileStatement<T>(T statement) where T : IStatement;
    }
}