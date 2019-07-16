namespace Llama.Parser.Framework
{
    public interface IErrorWithConfidence : IError
    {
        int ConfidenceMetric { get; }
    }
}