namespace Llama.Parser.Tokens
{
    using Framework;

    public class VariableDeclarationToken : TokenBase<VariableDeclarationToken>, IStatementToken
    {
        public readonly IdentifierToken DeclarationName;
        public readonly IdentifierToken DeclarationType;

        public VariableDeclarationToken(IdentifierToken declarationType, IdentifierToken declarationName)
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