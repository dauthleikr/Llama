namespace Llama.Compiler
{
    using Parser.Nodes;

    public interface ICompileStatements<in T> where T : IStatement
    {
        void Compile(T statement, ICompilationContext context);
    }
}