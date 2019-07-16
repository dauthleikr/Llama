namespace Llama.Parser.Entities
{
    public class CommaEntity : AtomicEntity<CommaEntity>
    {
        protected override string ToStringInternal() => ",";
    }
}