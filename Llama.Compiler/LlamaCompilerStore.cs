namespace Llama.Compiler
{
    using System;
    using ExpressionCompilers;
    using Parser.Nodes;
    using StatementCompilers;

    public sealed class LlamaCompilerStore : ICompilerStore
    {
        private struct ExpressionCompilerStore<T> where T : IExpression
        {
            public static ICompileExpressions<T> Compiler;
        }

        private struct StatementCompilerStore<T> where T : IStatement
        {
            public static ICompileStatements<T> Compiler;
        }

        public static LlamaCompilerStore Instance { get; } = new LlamaCompilerStore();

        static LlamaCompilerStore()
        {
            ExpressionCompilerStore<IExpression>.Compiler = new GenericExpressionCompiler();
            ExpressionCompilerStore<ArrayAllocationExpression>.Compiler = new ArrayAllocationCompiler();
            ExpressionCompilerStore<AtomicExpression>.Compiler = new AtomicExpressionCompiler();
            ExpressionCompilerStore<BinaryOperatorExpression>.Compiler = new BinaryOperationCompiler();
            ExpressionCompilerStore<ArrayAccessExpression>.Compiler = new ArrayAccessCompiler();
            ExpressionCompilerStore<MethodCallExpression>.Compiler = new MethodCallCompiler();
            ExpressionCompilerStore<TypeCastExpression>.Compiler = new TypeCastCompiler();

            StatementCompilerStore<CodeBlock>.Compiler = new CodeBlockCompiler();
            StatementCompilerStore<Declaration>.Compiler = new DeclarationCompiler();
            StatementCompilerStore<For>.Compiler = new ForCompiler();
            StatementCompilerStore<If>.Compiler = new IfCompiler();
            StatementCompilerStore<While>.Compiler = new WhileCompiler();
        }

        private LlamaCompilerStore() { }

        public ICompileExpressions<T> GetExpressionCompiler<T>() where T : IExpression => ExpressionCompilerStore<T>.Compiler;

        public ICompileStatements<T> GetStatementCompiler<T>() where T : IStatement => StatementCompilerStore<T>.Compiler;
    }
}