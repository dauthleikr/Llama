namespace Llama.Parser.Framework
{
    using System;

    public static class TokenizationResultExtensions
    {
        public static TA Match<T, TA>(this ITokenizationResult<T> result, Func<T, TA> success, Func<IErrorWithConfidence, ITokenizationResult<T>, TA> error) where T : class, IToken
        {
            return result.Successful ? success(result.ResultSuccess) : error(result.ResultError, result);
        }

        public static ITokenizationResult<T> Otherwise<T>(this ITokenizationResult<T> result, Func<ITokenizationResult<T>> next) where T : class, IToken
        {
            return result.Successful ? result : next();
        }

        public static ITokenizationResult<T> ImproveableWith<T>(this ITokenizationResult<T> result, Func<T, ITokenizationResult<T>> next) where T : class, IToken
        {
            if (!result.Successful)
                return result;

            var possibleImprovement = next(result.ResultSuccess);
            return possibleImprovement.Successful ? possibleImprovement : result;
        }
    }
}
