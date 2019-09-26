namespace Llama.Compiler
{
    using Parser.Nodes;
    using spit;

    public interface ICompileExpressions<in T> where T : IExpression
    {
        void Compile(
            T expression,
            Register target,
            CodeGen codeGen,
            IFunctionContext function,
            IAddressFixer addressFixer,
            ICompilationContext context
        );
    }
}