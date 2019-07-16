namespace Llama.Parser.Framework
{
    using System;
    using System.Collections.Generic;
    using Abstractions;
    using Entities;
    using Entities.Expressions;
    using Parsers;

    public class ParseStrategies : IParseStrategies
    {
        private readonly Dictionary<Type, object> _parsers = new Dictionary<Type, object>();

        public ParseStrategies()
        {
            Register<ExpressionParser, IExpressionEntity>();
            Register<BinaryOperatorParser, BinaryOperatorEntity>();
            Register<CloseParanthesisParser, CloseParanthesisEntity>();
            Register<OpenParanthesisParser, OpenParanthesisEntity>();
            Register<FunctionCallParser, FunctionCallEntity>();
            Register<IdentifierParser, IdentifierEntity>();
            Register<NumericLiteralParser, NumericLiteralEntity>();
            Register<CommaParser, CommaEntity>();
            Register<VariableDeclarationParser, VariableDeclarationEntity>();
            Register<AssignmentParser, AssignmentEntity>();
        }

        public IParse<T> GetStrategyFor<T>() where T : class, IEntity
        {
            IParse<T> result = null;
            if (_parsers.TryGetValue(typeof(T), out var resultObj))
                result = resultObj as IParse<T>;
            return result;
        }

        private void Register<T, TU>() where T : IParse<TU>, new() where TU : class, IEntity
        {
            _parsers.Add(typeof(TU), new T());
        }
    }
}