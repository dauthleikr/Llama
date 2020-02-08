namespace Llama.Compiler
{
    using Parser.Nodes;
    using spit;

    public interface ICompileExpressions<in T> where T : IExpression
    {
        /// <summary>
        ///     Compiles the expression. Compilers must prioritize returning pointers instead of dereferencing themselves.
        /// </summary>
        /// <param name="expression">The expression to compile</param>
        /// <param name="target">Where the result of the expression needs to be stored (preferrably)</param>
        /// <param name="codeGen"></param>
        /// <param name="storageManager">Manages non-volatile storage spaces (register or stack)</param>
        /// <param name="scope">Tracker for local variables. Compilers must declare their locals. Accesses have to be declared</param>
        /// <param name="linkingInfo">
        ///     For fixing offsets retroactively, as the compiler does not know where its code will be
        ///     placed in memory
        /// </param>
        /// <param name="context">For handing off compilation to other compilers for different expressions further down the tree</param>
        /// <returns></returns>
        ExpressionResult Compile(
            T expression,
            PreferredRegister target,
            CodeGen codeGen,
            StorageManager storageManager,
            ISymbolResolver scope,
            ILinkingInfo linkingInfo,
            ICompilationContext context
        );
    }
}