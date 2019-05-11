using System;
using System.Collections.Generic;
using System.Text;

namespace Llama.Parser.Tokens.Expressions
{
    using Framework;

    class FunctionCallFullToken : TokenBase<FunctionCallFullToken>, IExpressionToken
    {
        public readonly IExpressionToken ExpressionToken;
        public readonly FunctionCallToken FunctionCallToken;

        public FunctionCallFullToken(IExpressionToken expression, FunctionCallToken call)
        {
            ExpressionToken = expression;
            FunctionCallToken = call;
        }

        public override void WalkRecursive(ISourceWalker walker, bool codeChildrenOnly = true)
        {
            walker.Walk(this);
            ExpressionToken.WalkRecursive(walker, codeChildrenOnly);
            FunctionCallToken.WalkRecursive(walker, codeChildrenOnly);
        }

        public override string ToString() => $"{ExpressionToken}{FunctionCallToken}";
    }
}
