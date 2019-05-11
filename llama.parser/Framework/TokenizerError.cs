namespace Llama.Parser.Framework
{
    internal class TokenizerError : IErrorWithConfidence
    {
        public int ConfidenceMetric { get; }
        public string Message { get; }
        public long Index { get; }
        public int Length { get; }

        public TokenizerError(int confidenceMetric, string message, long index, int length = 0)
        {
            ConfidenceMetric = confidenceMetric;
            Message = message;
            Index = index;
            Length = length;
        }

        public TokenizerError(IError error, int confidenceMetric) : this(confidenceMetric, error.Message, error.Index, error.Length) { }
    }
}