namespace Llama.Parser.Entities
{
    public class AssignmentEntity : AtomicEntity<AssignmentEntity>
    {
        protected override string ToStringInternal() => "=";
    }
}