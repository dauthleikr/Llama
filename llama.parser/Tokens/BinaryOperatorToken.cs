namespace Llama.Parser.Tokens
{
    using Language;

    public class BinaryOperatorToken : AtomicToken<BinaryOperatorToken>
    {
        public byte Precedence => BinaryOperators.EnumToPrecedence[Operator];
        public string OperatorText => BinaryOperators.EnumToOperator[Operator];

        public readonly BinaryOperator Operator;

        public BinaryOperatorToken(BinaryOperator @operator) => Operator = @operator;

        protected override string ToStringInternal() => OperatorText;
    }
}