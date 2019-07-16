namespace Llama.Parser.Entities.Expressions
{
    using Abstractions;
    using Framework;

    internal class FunctionCallFullEntity : EntityBase<FunctionCallFullEntity>, IExpressionEntity
    {
        public readonly IExpressionEntity ExpressionEntity;
        public readonly FunctionCallEntity FunctionCallEntity;

        public FunctionCallFullEntity(IExpressionEntity expression, FunctionCallEntity call)
        {
            ExpressionEntity = expression;
            FunctionCallEntity = call;
        }

        public override void WalkRecursive(ISourceWalker walker, bool codeChildrenOnly = true)
        {
            walker.Walk(this);
            ExpressionEntity.WalkRecursive(walker, codeChildrenOnly);
            FunctionCallEntity.WalkRecursive(walker, codeChildrenOnly);
        }

        public override string ToString() => $"{ExpressionEntity}{FunctionCallEntity}";
    }
}