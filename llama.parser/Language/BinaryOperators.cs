namespace Llama.Parser.Language
{
    using System.Collections.Generic;
    using System.Linq;

    public static class BinaryOperators
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
            {":", BinaryOperator.ConditionalColon}
        };

        internal static readonly IReadOnlyDictionary<BinaryOperator, byte> EnumToPrecedence = new Dictionary<BinaryOperator, byte>
        {
            {BinaryOperator.MemberAccess, 100},
            {BinaryOperator.Division, 98},
            {BinaryOperator.Multiplication, 98},
            {BinaryOperator.Remainder, 98},
            {BinaryOperator.Addition, 96},
            {BinaryOperator.Substraction, 96},
            {BinaryOperator.LeftShift, 94},
            {BinaryOperator.RightShift, 94},
            {BinaryOperator.LessThan, 92},
            {BinaryOperator.GreaterThan, 92},
            {BinaryOperator.LessOrEquals, 92},
            {BinaryOperator.GreaterOrEquals, 92},
            {BinaryOperator.TypeEquality, 92},
            {BinaryOperator.Equality, 90},
            {BinaryOperator.NonEquality, 90},
            {BinaryOperator.LogicalAnd, 88},
            {BinaryOperator.LogicalXor, 86},
            {BinaryOperator.LogicalOr, 84},
            {BinaryOperator.ConditionalAnd, 82},
            {BinaryOperator.ConditionalOr, 80},
            {BinaryOperator.ConditionalColon, 78},
            {BinaryOperator.ConditionalQuestionmark, 76},
            {BinaryOperator.Assignment, 49},
            {BinaryOperator.AssignmentAddition, 49},
            {BinaryOperator.AssignmentSubstraction, 49},
            {BinaryOperator.AssignmentDivision, 49},
            {BinaryOperator.AssignmentMultiplication, 49},
            {BinaryOperator.AssignmentRemainder, 49},
            {BinaryOperator.AssignmentLogicalAnd, 49},
            {BinaryOperator.AssignmentLogicalOr, 49},
            {BinaryOperator.AssignmentLogicalXor, 49},
            {BinaryOperator.AssignmentLeftShift, 49},
            {BinaryOperator.AssignmentRightShift, 49}
        };

        internal static readonly IReadOnlyDictionary<BinaryOperator, string> EnumToOperator = OperatorToEnum.ToDictionary(item => item.Value, item => item.Key);
    }
}