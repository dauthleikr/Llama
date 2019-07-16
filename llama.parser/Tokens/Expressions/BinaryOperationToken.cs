namespace Llama.Parser.Tokens.Expressions
{
    using Framework;

    internal class BinaryOperationToken : TokenBase<BinaryOperationToken>, IExpressionToken
    {
        public IExpressionToken Left { get; internal set; }
        public BinaryOperatorToken BinaryOperator { get; }
        public IExpressionToken Right { get; internal set; }

        public BinaryOperationToken(IExpressionToken left, BinaryOperatorToken binaryOperator, IExpressionToken right)
        {
            Left = left;
            BinaryOperator = binaryOperator;
            Right = right;
        }

        public override void WalkRecursive(ISourceWalker walker, bool codeChildrenOnly = true)
        {
            BinaryOperator.WalkRecursive(walker, codeChildrenOnly);
            Left.WalkRecursive(walker, codeChildrenOnly);
            Right.WalkRecursive(walker, codeChildrenOnly);
        }

        public override string ToString() => $"{Left}{BinaryOperator}{Right}";
    }
}