namespace Llama.Compiler.StatementCompilers
{
    using Parser.Nodes;
    using spit;

    internal class DeclarationCompiler : ICompileStatements<Declaration>
    {
        public void Compile(
            Declaration statement,
            ICompilationContext context
        )
        {
            context.Symbols.DefineLocal(statement.Identifier.RawText, statement.Type);
            if (statement.InitialValue == null)
            {
                context.Generator.Xor(Register64.RAX, Register64.RAX);
                context.Generator.MovToDereferenced(Register64.RSP, Register64.RAX, context.Symbols.GetLocalOffset(statement.Identifier.RawText));
            }
            else
            {
                var initialValueRegister = context.CompileExpression(statement.InitialValue, PreferredRegister.DefaultVolatile)
                    .ToRegister(statement.Type, context.Generator, context.Linking);

                context.Symbols.GetLocalReference(statement.Identifier.RawText)
                    .GenerateAssign(initialValueRegister, context.Generator, context.Linking);
            }
        }
    }
}