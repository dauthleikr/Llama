namespace Llama.Parser.Entities
{
    using Abstractions;

    public class VariableInitializationEntity : VariableDeclarationEntity, IStatementEntity
    {
        public readonly AssignmentEntity Assignment;
        public readonly IExpressionEntity Value;

        public VariableInitializationEntity(IdentifierEntity declarationType, IdentifierEntity declarationName, AssignmentEntity assignment, IExpressionEntity value) : base(declarationType, declarationName)
        {
            Assignment = assignment;
            Value = value;
        }

        public override void WalkRecursive(ISourceWalker walker, bool codeChildrenOnly = true)
        {
            walker.Walk(this);
            DeclarationType.WalkRecursive(walker, codeChildrenOnly);
            DeclarationName.WalkRecursive(walker, codeChildrenOnly);
            Assignment.WalkRecursive(walker, codeChildrenOnly);
            Value.WalkRecursive(walker, codeChildrenOnly);
        }
    }
}