namespace Llama.Compiler
{
    using Parser.Nodes;

    internal interface ICompilationContext
    {
        Type CompileExpression(IExpression expression, Register target, IFunctionContext function, IAddressFixer addressFixer);
        void CompileStatement(IStatement statement, IFunctionContext function, IAddressFixer addressFixer);
    }
}