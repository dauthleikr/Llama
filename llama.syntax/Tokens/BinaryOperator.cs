using System;
using System.Collections.Generic;
using System.Text;

namespace llama.syntax.Tokens
{
    public enum BinaryOperator
    {
        Addition,
        Substraction,
        Division,
        Multiplication,
        Remainder,
        LeftShift,
        RightShift,
        LessThan,
        GreaterThan,
        LessOrEquals,
        GreaterOrEquals,
        TypeEquality,
        Equality,
        NonEquality,
        LogicalAnd,
        LogicalOr,
        LogicalXor,
        ConditionalAnd,
        ConditionalOr,
        Assignment,
        AssignmentAddition,
        AssignmentSubstraction,
        AssignmentDivision,
        AssignmentMultiplication,
        AssignmentRemainder,
        AssignmentLogicalAnd,
        AssignmentLogicalOr,
        AssignmentLogicalXor,
        AssignmentLeftShift,
        AssignmentRightShift,
        MemberAccess,
        ConditionalQuestionmark,
        ConditionalColon
    }
}
