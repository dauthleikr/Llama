namespace Llama.Parser.Entities
{
    using Abstractions;
    using Framework;

    public class VariableDeclarationEntity : EntityBase<VariableDeclarationEntity>, IStatementEntity
    {
        public readonly IdentifierEntity DeclarationName;
        public readonly IdentifierEntity DeclarationType;

        public VariableDeclarationEntity(IdentifierEntity declarationType, IdentifierEntity declarationName)
        {
            DeclarationType = declarationType;
            DeclarationName = declarationName;
        }

        public override void WalkRecursive(ISourceWalker walker, bool codeChildrenOnly = true)
        {
            walker.Walk(this);
            DeclarationType.WalkRecursive(walker, codeChildrenOnly);
            DeclarationName.WalkRecursive(walker, codeChildrenOnly);
        }
    }
}