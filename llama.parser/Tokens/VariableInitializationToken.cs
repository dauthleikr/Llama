namespace Llama.Parser.Tokens
{
    using Framework;

    public class VariableInitializationToken : TokenBase<VariableInitializationToken>, IStatementToken
    {
        public readonly AssignmentToken Assignment;
        public readonly VariableDeclarationToken Declaration;
        public readonly IExpressionToken Value;

        public VariableInitializationToken(VariableDeclarationToken declaration, AssignmentToken assignment, IExpressionToken value)
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