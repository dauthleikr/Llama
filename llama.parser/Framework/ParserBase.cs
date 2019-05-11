namespace Llama.Parser.Framework
{
    public abstract class ParserBase<T> : IParse<T> where T : class, IToken
    {
        public abstract ITokenizationResult<T> TryReadToken(ISourceReader reader, IParseContext context, INonCodeParser nonCodeParser);

        protected ITokenizationResult<T> Error(ISourcePeeker source, string message, int confidenceMetric)
        {
            var length = 1;
            var peekChar = source.PeekFurther(length);
            while (char.IsLetter(peekChar) || char.IsDigit(peekChar) || peekChar == '_')
                peekChar = source.PeekFurther(++length);
            return Error(source, message, confidenceMetric, length);
        }

        protected ITokenizationResult<T> Error(ISourcePeeker source, string message, int confidenceMetric, int length)
        {
            return new ErrorResult<T>(message, source.Position, length, confidenceMetric);
        }

        protected ITokenizationResult<T> Error(long position, string message, int confidenceMetric, int length)
        {
            return new ErrorResult<T>(message, position, length, confidenceMetric);
        }

        protected ITokenizationResult<T> ErrorExpectedToken(ISourceReader source, int confidenceMetric = 0)
        {
            return Error(source, $"Expected {typeof(T).Name}", confidenceMetric);
        }

        public abstract bool IsPlausible(ISourcePeeker reader, IParseContext context);
    }
}
