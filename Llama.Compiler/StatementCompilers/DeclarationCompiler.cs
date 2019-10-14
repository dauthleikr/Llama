namespace Llama.Compiler.StatementCompilers
{
    using Parser.Nodes;
    using spit;

    internal class DeclarationCompiler : ICompileStatements<Declaration>
    {
        public void Compile(
            Declaration statement,
            CodeGen codeGen,
            StorageManager storageManager,
            IScopeContext scope,
            IAddressFixer addressFixer,
            ICompilationContext context
        )
        {
            scope.DefineLocal(statement.Identifier.RawText, statement.Type);
            if (statement.InitialValue == null)
            {
                codeGen.Xor(Register64.RAX, Register64.RAX);
                codeGen.MovToDereferenced(Register64.RSP, Register64.RAX, scope.GetLocalOffset(statement.Identifier.RawText));
            }
            else
            {
                var preferredRegister = new PreferredRegister(Register64.RAX, XmmRegister.XMM0);
                var initialValue = context.CompileExpression(statement.InitialValue, codeGen, storageManager, preferredRegister, scope);
                var initialValueRegister = preferredRegister.MakeFor(statement.Type);

                initialValue.GenerateMoveTo(initialValueRegister, statement.Type, codeGen, addressFixer);
                scope.GetLocalReference(statement.Identifier.RawText).GenerateAssign(initialValueRegister, codeGen, addressFixer);
            }
        }
    }
}