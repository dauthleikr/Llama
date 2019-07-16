namespace Llama.Parser.Framework
{
    using Abstractions;

    internal class ParseError : IErrorWithConfidence
    {
        public int ConfidenceMetric { get; }
        public string Message { get; }
        public long Index { get; }
        public int Length { get; }

        public ParseError(int confidenceMetric, string message, long index, int length = 0)
        {
            ConfidenceMetric = confidenceMetric;
            Message = message;
            Index = index;
            Length = length;
        }

        public ParseError(IError error, int confidenceMetric) : this(confidenceMetric, error.Message, error.Index, error.Length) { }
    }
}