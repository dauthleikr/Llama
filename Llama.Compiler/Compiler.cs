namespace Llama.Compiler
{
    using System;
    using System.Collections.Generic;
    using System.IO;
    using System.Linq;
    using Parser.Nodes;
    using spit;

    public class Compiler
    {
        private readonly CodeGen _codeGen;
        private readonly ICompilationContext _context;
        private readonly MemoryStream _rawStream = new MemoryStream();

        public Compiler(ICompilationContext context)
        {
            _context = context ?? throw new ArgumentNullException(nameof(context));
            _codeGen = new CodeGen(_rawStream);
        }

        public void AddFunction(FunctionImplementation function)
        {
            if (function == null)
                throw new ArgumentNullException(nameof(function));

            _codeGen.Write(
                Enumerable.Repeat((byte)0xCC, (int)(_codeGen.StreamPosition % 16)).ToArray()
            ); // 16-byte align function with int3 breakpoints
            var allStatements = GetStatements(function.Body).ToArray();
            var allExpressions = GetExpressions(function.Body).ToArray();

            var locals = allStatements.OfType<Declaration>().Select(item => item.Identifier).Distinct().ToArray();
            var numLocals = locals.Length;
            var maxCalleeParameters = 4; // R9, R8, RDX, RCX home is guaranteed
            foreach (var methodCallExpression in allExpressions.OfType<MethodCallExpression>())
                if (methodCallExpression.Parameters.Length > maxCalleeParameters)
                    maxCalleeParameters = methodCallExpression.Parameters.Length;

            var localVariableSpace = numLocals * 8;
            var calleeParameterSpace = maxCalleeParameters * 8;
            if ((localVariableSpace + calleeParameterSpace) % 16 == 0)
                localVariableSpace += 8; // Stack needs to be 16-byte aligned outside of function prologue

            var functionStackSpace = localVariableSpace + calleeParameterSpace;
            _codeGen.Sub(Register64.RSP, functionStackSpace);

            var functionContext = new FunctionContext(locals.Select(tok => tok.RawText).ToArray(), calleeParameterSpace, functionStackSpace);
            _context.CompileStatement(StatementAsBlock(function.Body), _codeGen, functionContext);
        }

        public byte[] Finish() => _rawStream.ToArray();

        private static IEnumerable<IExpression> GetExpressions(IStatement block)
        {
            foreach (var statement in GetStatements(block))
                switch (statement)
                {
                    case Declaration declaration:
                        if (declaration.InitialValue != null)
                            foreach (var expression in GetExpressions(declaration.InitialValue))
                                yield return expression;
                        break;
                    case For @for:
                        if (@for.Condition != null)
                            foreach (var expression in GetExpressions(@for.Condition))
                                yield return expression;
                        if (@for.Increment != null)
                            foreach (var expression in GetExpressions(@for.Increment))
                                yield return expression;
                        break;
                    case IExpression expr:
                        foreach (var expression in GetExpressions(expr))
                            yield return expression;
                        break;
                    case If @if:
                        foreach (var expression in GetExpressions(@if.Condition))
                            yield return expression;
                        break;
                    case While @while:
                        foreach (var expression in GetExpressions(@while))
                            yield return expression;
                        break;
                }
        }

        private static IEnumerable<IExpression> GetExpressions(IExpression expr)
        {
            yield return expr;
            switch (expr)
            {
                case ArrayAccessExpression arrayAccessExpression:
                    foreach (var subExpression in GetExpressions(arrayAccessExpression.Array))
                        yield return subExpression;
                    foreach (var subExpression in GetExpressions(arrayAccessExpression.Index))
                        yield return subExpression;
                    break;
                case ArrayAllocationExpression arrayAllocationExpression:
                    foreach (var subExpression in GetExpressions(arrayAllocationExpression.Count))
                        yield return subExpression;
                    break;
                case BinaryOperatorExpression binaryOperatorExpression:
                    foreach (var subExpression in GetExpressions(binaryOperatorExpression.Left))
                        yield return subExpression;
                    foreach (var subExpression in GetExpressions(binaryOperatorExpression.Right))
                        yield return subExpression;
                    break;
                case MethodCallExpression methodCallExpression:
                    foreach (var subExpression in GetExpressions(methodCallExpression.Expression))
                        yield return subExpression;
                    foreach (var paramExpression in methodCallExpression.Parameters)
                        foreach (var subExpression in GetExpressions(paramExpression))
                            yield return subExpression;
                    break;
                case TypeCastExpression typeCast:
                    foreach (var subExpression in GetExpressions(typeCast.CastExpression))
                        yield return subExpression;
                    break;
                case UnaryOperatorExpression unaryOperatorExpression:
                    foreach (var subExpression in GetExpressions(unaryOperatorExpression.Expression))
                        yield return subExpression;
                    break;
            }
        }

        private static CodeBlock StatementAsBlock(IStatement statement) => statement is CodeBlock block ? block : new CodeBlock(statement);

        private static IEnumerable<IStatement> GetStatements(IStatement block)
        {
            foreach (var statement in StatementAsBlock(block).Statements)
            {
                yield return statement;
                switch (statement)
                {
                    case CodeBlock codeBlock:
                        foreach (var codeBlockStatement in GetStatements(codeBlock))
                            yield return codeBlockStatement;
                        break;
                    case For @for:
                        yield return @for.Instruction;
                        if (@for.Instruction is CodeBlock forBlock)
                            foreach (var codeBlockStatement in GetStatements(forBlock))
                                yield return codeBlockStatement;
                        break;
                    case If @if:
                        if (@if.Instruction is CodeBlock ifBlock)
                            foreach (var codeBlockStatement in GetStatements(ifBlock))
                                yield return codeBlockStatement;
                        if (@if.ElseInstruction != null && @if.ElseInstruction is CodeBlock elseBlock)
                            foreach (var codeBlockStatement in GetStatements(elseBlock))
                                yield return codeBlockStatement;
                        break;
                    case While @while:
                        if (@while.Instruction is CodeBlock whileBlock)
                            foreach (var codeBlockStatement in GetStatements(whileBlock))
                                yield return codeBlockStatement;
                        break;
                }
            }
        }
    }
}