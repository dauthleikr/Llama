namespace Llama.Compiler
{
    using Parser.Nodes;

    public interface ICompilerStore
    {
        ICompileExpressions<T> GetExpressionCompiler<T>() where T : IExpression;
        ICompileStatements<T> GetStatementCompiler<T>() where T : IStatement;
    }
}