namespace Llama.Compiler.StatementCompilers
{
    using System.Linq;
    using Extensions;
    using Parser.Nodes;
    using spit;

    internal class ForCompiler : ICompileStatements<For>
    {
        public void Compile(
            For statement,
            CodeGen codeGen,
            StorageManager storageManager,
            IScopeContext scope,
            IAddressFixer addressFixer,
            ICompilationContext context
        )
        {
            scope.PushScope();
            context.CompileStatement(statement.Variable, codeGen, storageManager, scope);

            var bodyAndIncrement = statement.Instruction.StatementAsBlock().Statements.Concat(new[] { statement.Increment }).ToArray();
            var equalWhile = new While(statement.Condition, new CodeBlock(bodyAndIncrement));
            context.CompileStatement(equalWhile, codeGen, storageManager, scope);
            scope.PopScope();
        }
    }
}