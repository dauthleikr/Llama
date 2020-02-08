﻿namespace Llama.Compiler
{
    using Parser.Nodes;
    using spit;

    public interface ICompileStatements<in T> where T : IStatement
    {
        void Compile(
            T statement,
            CodeGen codeGen,
            StorageManager storageManager,
            ISymbolResolver scope,
            ILinkingInfo linkingInfo,
            ICompilationContext context
        );
    }
}