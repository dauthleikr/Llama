namespace Llama.Parser.Framework
{
    using Abstractions;

    public class ErrorResult<T> : IErrorWithConfidence, IParseResult<T> where T : class, IEntity
    {
        public string Message { get; }
        public long Index { get; }
        public int Length { get; }
        public int ConfidenceMetric { get; }
        public bool Successful => false;
        public IErrorWithConfidence ResultError => this;
        public T ResultSuccess => null;

        public ErrorResult(string message, long index, int length, int confidenceMetric)
        {
            Message = message;
            Index = index;
            Length = length;
            ConfidenceMetric = confidenceMetric;
        }

        public ErrorResult(IError error, int confidence) : this(error.Message, error.Index, error.Length, confidence) { }
        public ErrorResult(IError error, string message, int confidence) : this(message, error.Index, error.Length, confidence) { }
    }
}