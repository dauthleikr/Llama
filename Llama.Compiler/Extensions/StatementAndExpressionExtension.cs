namespace Llama.Compiler.Extensions
{
    using System.Collections.Generic;
    using Parser.Nodes;

    internal static class StatementAndExpressionExtension
    {
        public static IEnumerable<IExpression> GetExpressions(this IStatement block)
        {
            foreach (var statement in GetStatements(block))
            {
                switch (statement)
                {
                    case Declaration declaration:
                        if (declaration.InitialValue != null)
                        {
                            foreach (var expression in GetExpressions(declaration.InitialValue))
                                yield return expression;
                        }

                        break;
                    case For @for:
                        if (@for.Condition != null)
                        {
                            foreach (var expression in GetExpressions(@for.Condition))
                                yield return expression;
                        }

                        if (@for.Increment != null)
                        {
                            foreach (var expression in GetExpressions(@for.Increment))
                                yield return expression;
                        }

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
                        foreach (var expression in GetExpressions(@while.Condition))
                            yield return expression;
                        break;
                }
            }
        }

        public static IEnumerable<IExpression> GetExpressions(this IExpression expr)
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
                    {
                        foreach (var subExpression in GetExpressions(paramExpression))
                            yield return subExpression;
                    }

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

        public static CodeBlock StatementAsBlock(this IStatement statement) => statement is CodeBlock block ? block : new CodeBlock(statement);

        public static IEnumerable<IStatement> GetStatements(this IStatement block)
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
                        {
                            foreach (var codeBlockStatement in GetStatements(new CodeBlock(forBlock.Statements)))
                                yield return codeBlockStatement;
                        }

                        break;
                    case If @if:
                        if (@if.Instruction is CodeBlock ifBlock)
                        {
                            foreach (var codeBlockStatement in GetStatements(new CodeBlock(ifBlock.Statements)))
                                yield return codeBlockStatement;
                        }

                        if (@if.ElseInstruction != null && @if.ElseInstruction is CodeBlock elseBlock)
                        {
                            foreach (var codeBlockStatement in GetStatements(new CodeBlock(elseBlock.Statements)))
                                yield return codeBlockStatement;
                        }

                        break;
                    case While @while:
                        if (@while.Instruction is CodeBlock whileBlock)
                        {
                            foreach (var codeBlockStatement in GetStatements(new CodeBlock(whileBlock.Statements)))
                                yield return codeBlockStatement;
                        }

                        break;
                }
            }
        }
    }
}