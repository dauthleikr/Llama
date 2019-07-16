namespace Llama.Parser.NonCode.Entities
{
    public class InvalidSyntaxNonCodeEntity : AtomicNonCodeEntity
    {
        public readonly string InvalidCode;

        public InvalidSyntaxNonCodeEntity(string invalidCode) => InvalidCode = invalidCode;

        public override string ToString() => InvalidCode;
    }
}