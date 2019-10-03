namespace Llama.Compiler.StatementCompilers
{
    using Parser.Nodes;
    using spit;

    internal class CodeBlockCompiler : ICompileStatements<CodeBlock>
    {
        public void Compile(CodeBlock statement, CodeGen codeGen, IScopeContext scope, IAddressFixer addressFixer, ICompilationContext context)
        {
            foreach (var subStatement in statement.Statements)
            {
                switch (subStatement)
                {
                    case CodeBlock codeBlock:
                        Compile(codeBlock, codeGen, scope, addressFixer, context);
                        break;
                    case Declaration declaration:
                        context.CompileStatement(declaration, codeGen, scope);
                        break;
                    case For @for:
                        context.CompileStatement(@for, codeGen, scope);
                        break;
                    case If @if:
                        context.CompileStatement(@if, codeGen, scope);
                        break;
                    case While @while:
                        context.CompileStatement(@while, codeGen, scope);
                        break;
                    case IExpression expression:
                        CompileExpression(expression, codeGen, context, scope);
                        break;
                }
            }
        }

        private static void CompileExpression<T>(T expression, CodeGen codeGen, ICompilationContext context, IScopeContext scope)
            where T : IExpression =>
            context.CompileExpression(expression, codeGen, new Register(Register64.RAX, XmmRegister.XMM0), scope);
    }
}