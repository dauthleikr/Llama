using System.Collections.Generic;

namespace llama.syntax.Tokens
{
    using System.Linq;

    public class BinaryOperatorToken : TokenBase
    {
        public static readonly IReadOnlyDictionary<string, BinaryOperator> OperatorToEnum = new Dictionary<string, BinaryOperator>
        {
            {"+", BinaryOperator.Addition},
            {"-", BinaryOperator.Substraction},
            {"/", BinaryOperator.Division},
            {"*", BinaryOperator.Multiplication},
            {"%", BinaryOperator.Remainder},
            {"<<", BinaryOperator.LeftShift},
            {">>", BinaryOperator.RightShift},
            {"<", BinaryOperator.LessThan},
            {">", BinaryOperator.GreaterThan},
            {"<=", BinaryOperator.LessOrEquals},
            {">=", BinaryOperator.GreaterOrEquals},
            {"is", BinaryOperator.TypeEquality},
            {"==", BinaryOperator.Equality},
            {"!=", BinaryOperator.NonEquality},
            {"&", BinaryOperator.LogicalAnd},
            {"|", BinaryOperator.LogicalOr},
            {"^", BinaryOperator.LogicalXor},
            {"&&", BinaryOperator.ConditionalAnd},
            {"||", BinaryOperator.ConditionalOr},
            {"=", BinaryOperator.Assignment},
            {"+=", BinaryOperator.AssignmentAddition},
            {"-=", BinaryOperator.AssignmentSubstraction},
            {"/=", BinaryOperator.AssignmentDivision},
            {"*=", BinaryOperator.AssignmentMultiplication},
            {"%=", BinaryOperator.AssignmentRemainder},
            {"&=", BinaryOperator.AssignmentLogicalAnd},
            {"|=", BinaryOperator.AssignmentLogicalOr},
            {"^=", BinaryOperator.AssignmentLogicalXor},
            {"<<=", BinaryOperator.AssignmentLeftShift},
            {">>=", BinaryOperator.AssignmentRightShift},
            {".", BinaryOperator.MemberAccess},
            {"?", BinaryOperator.ConditionalQuestionmark},
            {":", BinaryOperator.ConditionalColon},
        };

        public static readonly IReadOnlyDictionary<BinaryOperator, byte> OperatorToPrecedence = new Dictionary<BinaryOperator, byte>
        {
            {BinaryOperator.MemberAccess, 100},
            {BinaryOperator.Division, 99},
            {BinaryOperator.Multiplication, 99},
            {BinaryOperator.Remainder, 99},
            {BinaryOperator.Addition, 98},
            {BinaryOperator.Substraction, 98},
            {BinaryOperator.LeftShift, 97},
            {BinaryOperator.RightShift, 97},
            {BinaryOperator.LessThan, 96},
            {BinaryOperator.GreaterThan, 96},
            {BinaryOperator.LessOrEquals, 96},
            {BinaryOperator.GreaterOrEquals, 96},
            {BinaryOperator.TypeEquality, 96},
            {BinaryOperator.Equality, 95},
            {BinaryOperator.NonEquality, 95},
            {BinaryOperator.LogicalAnd, 94},
            {BinaryOperator.LogicalXor, 93},
            {BinaryOperator.LogicalOr, 92},
            {BinaryOperator.ConditionalAnd, 91},
            {BinaryOperator.ConditionalOr, 90},
            {BinaryOperator.ConditionalColon, 89},
            {BinaryOperator.ConditionalQuestionmark, 88},
            {BinaryOperator.Assignment, 49 },
            {BinaryOperator.AssignmentAddition, 49},
            {BinaryOperator.AssignmentSubstraction, 49},
            {BinaryOperator.AssignmentDivision, 49},
            {BinaryOperator.AssignmentMultiplication, 49},
            {BinaryOperator.AssignmentRemainder, 49},
            {BinaryOperator.AssignmentLogicalAnd, 49},
            {BinaryOperator.AssignmentLogicalOr, 49},
            {BinaryOperator.AssignmentLogicalXor, 49},
            {BinaryOperator.AssignmentLeftShift, 49},
            {BinaryOperator.AssignmentRightShift, 49},
        };


        public static readonly IReadOnlyDictionary<BinaryOperator, string> EnumToOperator = OperatorToEnum.ToDictionary(item => item.Value, item => item.Key);
        private static readonly HashSet<string> Operators = OperatorToEnum.Keys.ToHashSet();
        private static readonly HashSet<char> ValidFirstOperatorChars = OperatorToEnum.Select(item => item.Key.First()).ToHashSet();
        private static readonly int MaxOperatorLength = Operators.Max(k => k.Length);

        public byte Precedence => OperatorToPrecedence[Operator];
        public string OperatorText => EnumToOperator[Operator];

        public readonly BinaryOperator Operator;

        public BinaryOperatorToken(BinaryOperator @operator) => Operator = @operator;

        public override void WriteSourceRecursive(ITextOutput textOutput, bool codeOnly = true) => WriteSource(textOutput, OperatorText, !codeOnly);

        public static bool TryParse(ISourceReader reader, out BinaryOperatorToken result)
        {
            var @operator = reader.TryReadLongest(Operators, MaxOperatorLength);
            if (@operator == null)
            {
                result = null;
                return false;
            }

            result = new BinaryOperatorToken(OperatorToEnum[@operator]) { PostNonCodeToken = NonCodeToken.ParseOrNull(reader) };
            return true;
        }

        public static bool MayStartWith(char character) => ValidFirstOperatorChars.Contains(character);
    }
}
