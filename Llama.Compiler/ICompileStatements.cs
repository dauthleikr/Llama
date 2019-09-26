namespace Llama.Compiler
{
    using Parser.Nodes;

    internal interface ICompileStatements<in T> where T : IStatement
    {
        void Compile(T statement, IFunctionContext function, IAddressFixer addressFixer);
    }
}