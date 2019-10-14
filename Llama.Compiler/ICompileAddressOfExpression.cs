﻿namespace Llama.Compiler
{
    using Parser.Nodes;
    using spit;

    public interface ICompileAddressOfExpression<in T> where T : IExpression
    {
        Type Compile(T expression, Register target, CodeGen codeGen, IScopeContext scope, IAddressFixer addressFixer, ICompilationContext context);
    }
}