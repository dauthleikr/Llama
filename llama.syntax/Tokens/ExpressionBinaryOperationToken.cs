using System;
using System.Collections.Generic;
using System.Text;

namespace llama.syntax.Tokens
{
    public class ExpressionBinaryOperationToken : ExpressionToken
    {
        public ExpressionToken Left { get; internal set; }
        public BinaryOperatorToken BinaryOperator { get; }
        public ExpressionToken Right { get; }

        public ExpressionBinaryOperationToken(ExpressionToken left, BinaryOperatorToken binaryOperator, ExpressionToken right)
        {
            Left = left;
            BinaryOperator = binaryOperator;
            Right = right;

            if (BinaryOperator.Operator == Tokens.BinaryOperator.MemberAccess && !(right is ExpressionIdentifierToken))
                throw new ArgumentException($"{nameof(right)} may only be a hardcoded identifier if the operator is {nameof(Tokens.BinaryOperator.MemberAccess)}");
        }

        public override void WalkRecursive(ISourceWalker walker, bool codeChildrenOnly = true)
        {
            Left.WalkRecursive(walker, codeChildrenOnly);
            Right.WalkRecursive(walker, codeChildrenOnly);
            BinaryOperator.WalkRecursive(walker, codeChildrenOnly);
            base.WalkRecursive(walker, codeChildrenOnly);
        }

        public override void WriteSourceRecursive(ITextOutput textOutput, bool codeOnly = true)
        {
            Left.WriteSourceRecursive(textOutput, codeOnly);
            BinaryOperator.WriteSourceRecursive(textOutput, codeOnly);
            Right.WriteSourceRecursive(textOutput, codeOnly);
        }
    }
}
