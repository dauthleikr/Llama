using System;
using System.Collections.Generic;
using System.Text;

namespace Llama.Parser.Framework
{
    using System.Linq;
    using System.Reflection;
    using Parsers;
    using Tokens;
    using Tokens.Expressions;

    public class ParseStrategies : IParseStrategies
    {
        private readonly Dictionary<Type, object> _parsers = new Dictionary<Type, object>();

        public ParseStrategies()
        {
            Register<ExpressionParser, IExpressionToken>();
            Register<BinaryOperatorParser, BinaryOperatorToken>();
            Register<CloseParanthesisParser, CloseParanthesisToken>();
            Register<OpenParanthesisParser, OpenParanthesisToken>();
            Register<FunctionCallParser, FunctionCallToken>();
            Register<IdentifierParser, IdentifierToken>();
            Register<NumericLiteralParser, NumericLiteralToken>();
            Register<CommaParser, CommaToken>();
        }

        private void Register<T, TU>() where T : IParse<TU>, new() where TU : class, IToken
        {
            _parsers.Add(typeof(TU), new T());
        }

        public IParse<T> GetStrategyFor<T>() where T : class, IToken
        {
            IParse<T> result = null;
            if (_parsers.TryGetValue(typeof(T), out var resultObj))
                result = resultObj as IParse<T>;
            return result;
        }
    }
}
