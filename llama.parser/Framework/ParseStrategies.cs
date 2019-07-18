namespace Llama.Parser.Framework
{
    using Abstractions;
    using Entities;
    using Entities.Expressions;
    using Parsers;

    public class ParseStrategies : IParseStrategies
    {
        private struct StrategyStore<T> where T : class, IEntity
        {
            public static IParse<T> Strategy;
        }

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

        public IParse<T> GetStrategyFor<T>() where T : class, IEntity => StrategyStore<T>.Strategy;

        private static void Register<T, TU>() where T : IParse<TU>, new() where TU : class, IEntity
        {
            StrategyStore<TU>.Strategy = new T();
        }
    }
}