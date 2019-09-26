namespace Llama.Compiler
{
    using Parser.Nodes;

    internal interface ICompilationContext
    {
        Type CompileExpression<T>(T expression, Register target, IFunctionContext function) where T : IExpression;
        void CompileStatement<T>(T statement, IFunctionContext function) where T : IStatement;
    }
}