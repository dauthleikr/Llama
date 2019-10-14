namespace Llama.Compiler.StatementCompilers
{
    using Parser.Nodes;
    using spit;

    internal class CodeBlockCompiler : ICompileStatements<CodeBlock>
    {
        public void Compile(CodeBlock statement, CodeGen codeGen, StorageManager storageManager, IScopeContext scope, IAddressFixer addressFixer, ICompilationContext context)
        {
            scope.PushScope();
            foreach (var subStatement in statement.Statements)
            {
                switch (subStatement)
                {
                    case CodeBlock codeBlock:
                        Compile(codeBlock, codeGen, storageManager, scope, addressFixer, context);
                        break;
                    case Declaration declaration:
                        context.CompileStatement(declaration, codeGen, storageManager, scope);
                        break;
                    case For @for:
                        context.CompileStatement(@for, codeGen, storageManager, scope);
                        break;
                    case If @if:
                        context.CompileStatement(@if, codeGen, storageManager, scope);
                        break;
                    case While @while:
                        context.CompileStatement(@while, codeGen, storageManager, scope);
                        break;
                    case IExpression expression:
                        context.CompileExpression(expression, codeGen, storageManager, new PreferredRegister(Register64.RAX, XmmRegister.XMM0), scope);
                        break;
                }
            }
            scope.PopScope();
        }
    }
}