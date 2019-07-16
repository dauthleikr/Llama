namespace Llama.Parser.Entities
{
    public class IdentifierEntity : AtomicEntity<IdentifierEntity>, IExpressionEntity
    {
        public readonly string Identifier;

        public IdentifierEntity(string identifier) => Identifier = identifier;

        protected override string ToStringInternal() => Identifier;
    }
}