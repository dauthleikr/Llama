namespace Llama.Compiler.ExpressionCompilers
{
    using System;
    using Parser.Nodes;
    using spit;
    using Type = Parser.Nodes.Type;

    internal class GenericExpressionCompiler : ICompileExpressions<IExpression>
    {
        public Type Compile(
            IExpression expression,
            Register target,
            CodeGen codeGen,
            IScopeContext scope,
            IAddressFixer addressFixer,
            ICompilationContext context
        ) =>
            expression switch
            {
                ArrayAccessExpression arrayAccessExpression         => context.CompileExpression(arrayAccessExpression, codeGen, target, scope),
                ArrayAllocationExpression arrayAllocationExpression => context.CompileExpression(arrayAllocationExpression, codeGen, target, scope),
                AtomicExpression atomicExpression                   => context.CompileExpression(atomicExpression, codeGen, target, scope),
                BinaryOperatorExpression binaryOperatorExpression   => context.CompileExpression(binaryOperatorExpression, codeGen, target, scope),
                MethodCallExpression methodCallExpression           => context.CompileExpression(methodCallExpression, codeGen, target, scope),
                TypeCastExpression typeCastExpression               => context.CompileExpression(typeCastExpression, codeGen, target, scope),
                UnaryOperatorExpression unaryOperatorExpression     => context.CompileExpression(unaryOperatorExpression, codeGen, target, scope),
                _ => throw new NotImplementedException(
                    $"Compiler for expression type \"{expression.GetType().Name}\" has not yet been implemented"
                )
            };
    }
}