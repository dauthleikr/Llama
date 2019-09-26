namespace Llama.Compiler
{
    using Parser.Nodes;

    internal interface ICompileExpressions<in T> where T : IExpression
    {
        Type Compile(T expression, Register target, IFunctionContext function, IAddressFixer addressFixer, ICompilationContext context);
    }
}