namespace Llama.Parser.Entities
{
    using Abstractions;
    using Framework;

    public class VariableInitializationEntity : EntityBase<VariableInitializationEntity>, IStatementEntity
    {
        public readonly AssignmentEntity Assignment;
        public readonly VariableDeclarationEntity Declaration;
        public readonly IExpressionEntity Value;

        public VariableInitializationEntity(VariableDeclarationEntity declaration, AssignmentEntity assignment, IExpressionEntity value)
        {
            Declaration = declaration;
            Assignment = assignment;
            Value = value;
        }

        public override void WalkRecursive(ISourceWalker walker, bool codeChildrenOnly = true)
        {
            walker.Walk(this);
        }
    }
}