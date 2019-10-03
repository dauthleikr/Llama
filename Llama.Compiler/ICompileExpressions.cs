namespace Llama.Compiler
{
    using Parser.Nodes;
    using spit;

    public interface ICompileExpressions<in T> where T : IExpression
    {
        Type Compile(T expression, Register target, CodeGen codeGen, IScopeContext scope, IAddressFixer addressFixer, ICompilationContext context);
    }
}