namespace Llama.Compiler.ExpressionCompilers
{
    using System;
    using System.Text;
    using Parser.Lexer;
    using Parser.Nodes;
    using spit;
    using Type = Parser.Nodes.Type;

    internal class AtomicExpressionCompiler : ICompileExpressions<AtomicExpression>
    {
        public ExpressionResult Compile(
            AtomicExpression expression,
            PreferredRegister target,
            CodeGen codeGen,
            StorageManager storageManager,
            IScopeContext scope,
            IAddressFixer addressFixer,
            ICompilationContext context
        )
        {
            if (expression.Token.Kind == TokenKind.Identifier)
                return CompileIdentifier(expression, codeGen, scope);
            if (expression.Token.Kind == TokenKind.FloatLiteral)
                return CompileFloatLiteral(expression);
            if (expression.Token.Kind == TokenKind.IntegerLiteral)
                return CompileIntegerLiteral(expression);
            if (expression.Token.Kind == TokenKind.StringLiteral)
                return CompileStringLiteral(expression, codeGen, target, addressFixer);

            throw new NotImplementedException($"Atomic expression type {expression.Token.Kind} not implemented");
        }

        private static ExpressionResult CompileFloatLiteral(AtomicExpression expression)
        {
            if (!double.TryParse(expression.Token.RawText, out var result))
                throw new BadLiteralException(expression.Token.RawText);

            return new ExpressionResult(Constants.DoubleType, (fixer, gen) => fixer.FixConstantDataOffset(gen, BitConverter.GetBytes(result)));
        }

        private static ExpressionResult CompileIntegerLiteral(AtomicExpression expression)
        {
            if (!long.TryParse(expression.Token.RawText, out var result))
                throw new BadLiteralException(expression.Token.RawText);

            return new ExpressionResult(
                GetMostNarrowSignedType(result),
                (fixer, gen) => fixer.FixConstantDataOffset(gen, BitConverter.GetBytes(result))
            );
        }

        private static ExpressionResult CompileStringLiteral(
            AtomicExpression expression,
            CodeGen codeGen,
            PreferredRegister target,
            IAddressFixer fixer
        )
        {
            var literalContent = expression.Token.RawText.Substring(1, expression.Token.RawText.Length - 2);
            literalContent = literalContent.Replace(@"\n", "\n");
            literalContent = literalContent.Replace(@"\r", "\r");
            literalContent = literalContent.Replace(@"\t", "\t");
            literalContent = literalContent.Replace(@"\'", "\'");
            literalContent = literalContent.Replace(@"\""", "\"");
            literalContent = literalContent.Replace(@"\0", "\0");
            // literalContent = literalContent.Replace(@"\a", "\a");
            // literalContent = literalContent.Replace(@"\b", "\b");
            // literalContent = literalContent.Replace(@"\f", "\f");

            var literalBytes = Encoding.ASCII.GetBytes(literalContent + "\0");
            var targetRegister = target.MakeFor(Constants.CstrType);
            codeGen.LeaFromDereferenced4(targetRegister, Constants.DummyOffsetInt);
            fixer.FixConstantDataOffset(codeGen, literalBytes);
            return new ExpressionResult(Constants.CstrType, targetRegister);
        }

        private static ExpressionResult CompileIdentifier(AtomicExpression expression, CodeGen codeGen, IScopeContext scope)
        {
            if (scope.IsLocalDefined(expression.Token.RawText))
            {
                var localType = scope.GetLocalType(expression.Token.RawText);
                return new ExpressionResult(localType, Register64.RSP, scope.GetLocalOffset(expression.Token.RawText));
            }

            // it should be a function pointer
            return new ExpressionResult(Constants.FunctionPointerType, (fixer, gen) => fixer.FixFunctionAddress(codeGen, expression.Token.RawText));
        }

        private static Type GetMostNarrowSignedType(long value)
        {
            if (value <= sbyte.MaxValue && value >= sbyte.MinValue)
                return Constants.SbyteType;
            if (value <= short.MaxValue && value >= short.MinValue)
                return Constants.SbyteType;
            if (value <= int.MaxValue && value >= int.MinValue)
                return Constants.SbyteType;
            return Constants.LongType;
        }
    }
}