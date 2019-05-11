using System;
using System.Collections.Generic;
using System.Text;

namespace llama.syntax.Tokens
{
    public class VariableDeclarationWithAssignmentToken : VariableDeclarationToken
    {
        public readonly EqualsToken EqualsToken;
        public readonly ExpressionToken ExpressionToken;

        public VariableDeclarationWithAssignmentToken(ExpressionIdentifierToken type, ExpressionIdentifierToken name, EqualsToken equals, ExpressionToken initial) : base(type, name)
        {
            EqualsToken = equals;
            ExpressionToken = initial;
        }

        public override void WriteSourceRecursive(ITextOutput textOutput, bool codeOnly = true)
        {
            Type.WriteSourceRecursive(textOutput, codeOnly);
            Name.WriteSourceRecursive(textOutput, codeOnly);
        }

        public override void WalkRecursive(ISourceWalker walker, bool codeChildrenOnly = true)
        {
            ExpressionToken.WalkRecursive(walker, codeChildrenOnly);
            EqualsToken.WalkRecursive(walker, codeChildrenOnly);
            base.WalkRecursive(walker, codeChildrenOnly);
        }
    }
}
