namespace Llama.Parser.Entities.Expressions
{
    using Abstractions;
    using Framework;

    internal class BinaryOperationEntity : EntityBase<BinaryOperationEntity>, IExpressionEntity
    {
        public IExpressionEntity Left { get; internal set; }
        public BinaryOperatorEntity BinaryOperator { get; }
        public IExpressionEntity Right { get; internal set; }

        public BinaryOperationEntity(IExpressionEntity left, BinaryOperatorEntity binaryOperator, IExpressionEntity right)
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