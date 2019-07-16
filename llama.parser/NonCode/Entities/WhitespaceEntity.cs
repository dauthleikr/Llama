namespace Llama.Parser.NonCode.Entities
{
    public class WhitespaceEntity : AtomicNonCodeEntity
    {
        public readonly string Whitespaces;

        public WhitespaceEntity(string whitespaces) => Whitespaces = whitespaces;

        public override string ToString() => Whitespaces;
    }
}