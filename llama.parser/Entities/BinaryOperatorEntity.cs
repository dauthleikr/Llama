namespace Llama.Parser.Entities
{
    using Language;

    public class BinaryOperatorEntity : AtomicEntity<BinaryOperatorEntity>
    {
        public byte Precedence => BinaryOperators.EnumToPrecedence[Operator];
        public string OperatorText => BinaryOperators.EnumToOperator[Operator];

        public readonly BinaryOperator Operator;

        public BinaryOperatorEntity(BinaryOperator @operator) => Operator = @operator;

        protected override string ToStringInternal() => OperatorText;
    }
}