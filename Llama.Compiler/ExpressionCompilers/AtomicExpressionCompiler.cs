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
        public Type Compile(
            AtomicExpression expression,
            Register target,
            CodeGen codeGen,
            IScopeContext scope,
            IAddressFixer addressFixer,
            ICompilationContext context
        )
        {
            if (expression.Token.Kind == TokenKind.Identifier)
                return CompileIdentifier(expression, target, codeGen, scope, addressFixer);
            if (expression.Token.Kind == TokenKind.FloatLiteral)
                return CompileFloatLiteral(expression, target, codeGen);
            if (expression.Token.Kind == TokenKind.IntegerLiteral)
                return CompileIntegerLiteral(expression, target, codeGen);
            if (expression.Token.Kind == TokenKind.StringLiteral)
                return CompileStringLiteral(expression, target, codeGen, addressFixer);

            throw new NotImplementedException($"Atomic expression type {expression.Token.Kind} not implemented");
        }

        private static Type CompileFloatLiteral(AtomicExpression expression, Register target, CodeGen codeGen)
        {
            target.AssertIsFloat();
            if (!double.TryParse(expression.Token.RawText, out var result))
                throw new BadLiteralException(expression.Token.RawText);

            codeGen.Mov(Register64.RAX, BitConverter.ToInt64(BitConverter.GetBytes(result)));
            codeGen.Movq(target.FloatRegister, Register64.RAX);
            return Constants.DoubleType;
        }

        private static Type CompileIntegerLiteral(AtomicExpression expression, Register target, CodeGen codeGen)
        {
            target.AssertIsInteger();
            if (!long.TryParse(expression.Token.RawText, out var result))
                throw new BadLiteralException(expression.Token.RawText);

            if (result <= int.MaxValue && result >= int.MinValue)
                codeGen.Mov(target.IntegerRegister, (int)result);
            else
                codeGen.Mov(target.IntegerRegister, result);
            return GetMostNarrowSignedType(result);
        }

        private static Type CompileStringLiteral(AtomicExpression expression, Register target, CodeGen codeGen, IAddressFixer addressFixer)
        {
            target.AssertIsInteger();
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

            codeGen.Mov(target.IntegerRegister, Constants.DummyAddress);
            addressFixer.FixConstantDataAddress(codeGen, literalBytes);
            return Constants.CstrType;
        }

        private static Type CompileIdentifier(
            AtomicExpression expression,
            Register target,
            CodeGen codeGen,
            IScopeContext scope,
            IAddressFixer addressFixer
        )
        {
            if (scope.IsLocalDefined(expression.Token.RawText))
            {
                if (target.CanUseIntegerRegister)
                    codeGen.MovFromDereferenced(target.IntegerRegister, Register64.RSP, scope.GetLocalOffset(expression.Token.RawText), segment: Segment.SS);
                else
                    codeGen.MovsdFromDereferenced(target.FloatRegister, Register64.RSP, scope.GetLocalOffset(expression.Token.RawText), segment: Segment.SS);

                return scope.GetLocalType(expression.Token.RawText);
            }

            target.AssertIsInteger();
            codeGen.Mov(target.IntegerRegister, Constants.DummyAddress);
            addressFixer.FixFunctionAddress(codeGen, expression.Token.RawText);
            return Constants.FunctionPointerType;
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