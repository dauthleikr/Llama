﻿namespace Llama.Parser.Parsers
{
    using System.Collections.Generic;
    using Lexer;
    using Nodes;

    internal class ExpressionParser : IParse<IExpression>
    {
        public IExpression Read(IParseContext context)
        {
            var expression = ReadUncontinuedExpression(context);
            IExpression expressionContinued;
            while ((expressionContinued = TryContinueExpression(context, expression)) != expression)
                expression = expressionContinued;
            return expression;
        }

        public IExpression ReadShortExpression(IParseContext context)
        {
            var expression = ReadUncontinuedExpression(context);
            IExpression expressionContinued;
            while ((expressionContinued = TryContinueShortExpression(context, expression)) != expression)
                expression = expressionContinued;
            return expression;
        }

        private IExpression ReadUncontinuedExpression(IParseContext context)
        {
            if (context.NextCodeToken.Kind == TokenKind.OpenAngularBracket)
                return ReadTypeCastExpression(context);
            if (context.NextCodeToken.Kind == TokenKind.OpenParanthesis)
                return ReadParanthesisExpression(context);
            if (context.NextCodeToken.Kind == TokenKind.New)
                return ReadArrayAllocationExpression(context);
            if (UnaryOperator.IsTokenKindValid(context.NextCodeToken.Kind))
                return ReadUnaryOperatorExpression(context);
            return ReadAtomicExpression(context);
        }

        private IExpression TryContinueShortExpression(IParseContext context, IExpression currentExpression)
        {
            if (context.NextCodeToken.Kind == TokenKind.OpenParanthesis)
                return ReadFunctionCallExpression(context, currentExpression);
            if (context.NextCodeToken.Kind == TokenKind.OpenSquareBracket)
                return ReadArrayAccessExpression(context, currentExpression);
            return currentExpression;
        }

        private IExpression TryContinueExpression(IParseContext context, IExpression currentExpression)
        {
            if (BinaryOperator.IsTokenKindValid(context.NextCodeToken.Kind))
                return ReadBinaryOperatorExpression(context, currentExpression);
            return TryContinueShortExpression(context, currentExpression);
        }

        private IExpression ReadAtomicExpression(IParseContext context)
        {
            if (AtomicExpression.IsTokenKindValid(context.NextCodeToken.Kind))
                return new AtomicExpression(context.ReadCodeToken());

            context.Panic($"Expected {nameof(AtomicExpression)}, but got: {context.NextCodeToken}");
            return null;
        }

        private IExpression ReadArrayAllocationExpression(IParseContext context)
        {
            context.ReadOrPanic(TokenKind.New);
            var allocationType = context.ReadNode<Type>();
            context.ReadOrPanic(TokenKind.OpenParanthesis);
            var count = Read(context);
            context.ReadOrPanic(TokenKind.CloseParanthesis);
            return new ArrayAllocationExpression(allocationType, count);
        }

        private IExpression ReadUnaryOperatorExpression(IParseContext context)
        {
            var op = context.ReadNode<UnaryOperator>();
            return new UnaryOperatorExpression(op, ReadShortExpression(context));
        }

        private IExpression ReadTypeCastExpression(IParseContext context)
        {
            context.ReadOrPanic(TokenKind.OpenAngularBracket);
            var type = context.ReadNode<Type>();
            context.ReadOrPanic(TokenKind.CloseAngularBracket);
            return new TypeCastExpression(type, ReadShortExpression(context));
        }

        private IExpression ReadParanthesisExpression(IParseContext context)
        {
            context.ReadOrPanic(TokenKind.OpenParanthesis);
            var expression = Read(context);
            context.ReadOrPanic(TokenKind.CloseParanthesis);
            return expression;
        }

        private FunctionCallExpression ReadFunctionCallExpression(IParseContext context, IExpression called)
        {
            context.ReadOrPanic(TokenKind.OpenParanthesis);
            var parameters = new List<IExpression>();
            while (context.NextCodeToken.Kind != TokenKind.CloseParanthesis)
            {
                parameters.Add(Read(context));
                if (context.NextCodeToken.Kind == TokenKind.Comma)
                    context.ReadCodeToken();
            }

            context.ReadOrPanic(TokenKind.CloseParanthesis);
            return new FunctionCallExpression(called, parameters.ToArray());
        }

        private IExpression ReadArrayAccessExpression(IParseContext context, IExpression array)
        {
            context.ReadOrPanic(TokenKind.OpenSquareBracket);
            var indexExpression = Read(context);
            context.ReadOrPanic(TokenKind.CloseSquareBracket);
            return new ArrayAccessExpression(array, indexExpression);
        }

        private IExpression ReadBinaryOperatorExpression(IParseContext context, IExpression left)
        {
            var binaryOperator = context.ReadNode<BinaryOperator>();
            var right = Read(context);
            if (!(left is BinaryOperatorExpression rightBinaryExpression))
                return new BinaryOperatorExpression(left, binaryOperator, right);

            BinaryOperatorExpression binaryOperation = null;
            var precedenceDominated = GetPrecedenceDominatedEntity(rightBinaryExpression, binaryOperator.Precedence);
            if (precedenceDominated != null)
            {
                var newTornOperation = new BinaryOperatorExpression(left, binaryOperator, precedenceDominated.Left);
                precedenceDominated.Left = newTornOperation;
                binaryOperation = rightBinaryExpression;
            }

            return binaryOperation ?? new BinaryOperatorExpression(left, binaryOperator, right);
        }

        private static BinaryOperatorExpression GetPrecedenceDominatedEntity(BinaryOperatorExpression root, int precedence)
        {
            if (root.Operator.Precedence >= precedence)
                return null;

            if (root.Left is BinaryOperatorExpression binaryOperation)
                return GetPrecedenceDominatedEntity(binaryOperation, precedence) ?? root;
            return root;
        }
    }
}