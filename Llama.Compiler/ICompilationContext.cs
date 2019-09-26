namespace Llama.Compiler
{
    using Parser.Nodes;
    using spit;

    public interface ICompilationContext
    {
        Type CompileExpression<T>(T expression, CodeGen codeGen, Register target, IFunctionContext function) where T : IExpression;
        void CompileStatement<T>(T statement, CodeGen codeGen, IFunctionContext function) where T : IStatement;
    }
}