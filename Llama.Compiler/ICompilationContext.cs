namespace Llama.Compiler
{
    using Parser.Nodes;
    using spit;

    public interface ICompilationContext
    {
        Type CompileExpression<T>(T expression, CodeGen codeGen, Register target, IScopeContext scope) where T : IExpression;
        void CompileStatement<T>(T statement, CodeGen codeGen, IScopeContext scope) where T : IStatement;
    }
}