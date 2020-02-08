namespace Llama.Compiler.StatementCompilers
{
    using System;
    using Parser.Nodes;
    using spit;

    internal class CodeBlockCompiler : ICompileStatements<CodeBlock>
    {
        public void Compile(
            CodeBlock statement,
            ICompilationContext context
        )
        {
            context.Symbols.PushLocalScope();
            foreach (var subStatement in statement.Statements)
            {
                switch (subStatement)
                {
                    case CodeBlock codeBlock:
                        Compile(codeBlock, context);
                        break;
                    case Declaration declaration:
                        context.CompileStatement(declaration);
                        break;
                    case For @for:
                        context.CompileStatement(@for);
                        break;
                    case If @if:
                        context.CompileStatement(@if);
                        break;
                    case While @while:
                        context.CompileStatement(@while);
                        break;
                    case Return @return:
                        context.CompileStatement(@return);
                        break;
                    case IExpression expression:
                        context.CompileExpression(
                            expression,
                            new PreferredRegister(Register64.RAX, XmmRegister.XMM0)
                        );
                        break;
                    default:
                        throw new NotImplementedException(
                            $"{nameof(CodeBlockCompiler)}: I do not know how to compile: {subStatement.GetType().Name}"
                        );
                }
            }

            context.Symbols.PopLocalScope();
        }
    }
}