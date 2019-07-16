namespace Llama.Parser.Entities
{
    public class CloseParanthesisEntity : AtomicEntity<CloseParanthesisEntity>
    {
        protected override string ToStringInternal() => ")";
    }
}