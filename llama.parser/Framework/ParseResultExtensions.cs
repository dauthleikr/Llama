namespace Llama.Parser.Framework
{
    using System;
    using Abstractions;

    public static class ParseResultExtensions
    {
        public static TA Match<T, TA>(this IParseResult<T> result, Func<T, TA> success, Func<IErrorWithConfidence, IParseResult<T>, TA> error) where T : class, IEntity => result.Successful ? success(result.ResultSuccess) : error(result.ResultError, result);

        public static IParseResult<T> Otherwise<T>(this IParseResult<T> result, Func<IParseResult<T>> next) where T : class, IEntity => result.Successful ? result : next();

        public static IParseResult<T> ImproveableWith<T>(this IParseResult<T> result, Func<T, IParseResult<T>> next) where T : class, IEntity
        {
            if (!result.Successful)
                return result;

            var possibleImprovement = next(result.ResultSuccess);
            return possibleImprovement.Successful ? possibleImprovement : result;
        }
    }
}