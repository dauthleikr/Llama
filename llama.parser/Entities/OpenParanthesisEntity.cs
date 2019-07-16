namespace Llama.Parser.Entities
{
    public class OpenParanthesisEntity : AtomicEntity<OpenParanthesisEntity>
    {
        protected override string ToStringInternal() => "(";
    }
}