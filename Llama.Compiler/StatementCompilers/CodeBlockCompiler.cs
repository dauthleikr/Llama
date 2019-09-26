namespace Llama.Compiler.StatementCompilers
{
    using Parser.Nodes;
    using spit;

    internal class CodeBlockCompiler : ICompileStatements<CodeBlock>
    {
        public void Compile(CodeBlock statement, CodeGen codeGen, IFunctionContext function, IAddressFixer addressFixer, ICompilationContext context)
        {
            foreach (var subStatement in statement.Statements)
                switch (subStatement)
                {
                    case CodeBlock codeBlock:
                        Compile(codeBlock, codeGen, function, addressFixer, context);
                        break;
                    case ArrayAllocationExpression arrayAllocationExpression:
                        CompileExpression(arrayAllocationExpression, codeGen, context, function);
                        break;
                    case AtomicExpression atomicExpression:
                        CompileExpression(atomicExpression, codeGen, context, function);
                        break;
                    case BinaryOperatorExpression binaryOperatorExpression:
                        CompileExpression(binaryOperatorExpression, codeGen, context, function);
                        break;
                    case ArrayAccessExpression arrayAccessExpression:
                        CompileExpression(arrayAccessExpression, codeGen, context, function);
                        break;
                    case Declaration declaration:
                        context.CompileStatement(declaration, codeGen, function);
                        break;
                    case For @for:
                        context.CompileStatement(@for, codeGen, function);
                        break;
                    case MethodCallExpression methodCallExpression:
                        CompileExpression(methodCallExpression, codeGen, context, function);
                        break;
                    case TypeCastExpression typeCastExpression:
                        CompileExpression(typeCastExpression, codeGen, context, function);
                        break;
                    case UnaryOperatorExpression unaryOperatorExpression:
                        CompileExpression(unaryOperatorExpression, codeGen, context, function);
                        break;
                    case If @if:
                        context.CompileStatement(@if, codeGen, function);
                        break;
                    case While @while:
                        context.CompileStatement(@while, codeGen, function);
                        break;
                }
        }

        private static void CompileExpression<T>(T expression, CodeGen codeGen, ICompilationContext context, IFunctionContext function)
            where T : IExpression =>
            context.CompileExpression(expression, codeGen, new Register(Register64.RAX, XmmRegister.XMM0), function);
    }
}