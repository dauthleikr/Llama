namespace Llama.Parser.Framework
{
    using Abstractions;

    public abstract class ParserBase<T> : IParse<T> where T : class, IEntity
    {
        public abstract IParseResult<T> TryRead(ISourceReader reader, IParseContext context, INonCodeParser nonCodeParser);

        public abstract bool IsPlausible(ISourcePeeker reader, IParseContext context);

        protected IParseResult<T> Error(ISourcePeeker source, string message, int confidenceMetric)
        {
            var length = 1;
            var peekChar = source.PeekFurther(length);
            while (char.IsLetter(peekChar) || char.IsDigit(peekChar) || peekChar == '_')
                peekChar = source.PeekFurther(++length);
            return Error(source, message, confidenceMetric, length);
        }

        protected IParseResult<T> Error(ISourcePeeker source, string message, int confidenceMetric, int length) => new ErrorResult<T>(message, source.Position, length, confidenceMetric);

        protected IParseResult<T> Error(long position, string message, int confidenceMetric, int length) => new ErrorResult<T>(message, position, length, confidenceMetric);

        protected IParseResult<T> ErrorExpectedEntity(ISourceReader source, int confidenceMetric = 0) => Error(source, $"Expected {typeof(T).Name}", confidenceMetric);
    }
}