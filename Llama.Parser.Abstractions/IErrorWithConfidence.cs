namespace Llama.Parser.Abstractions
{
    public interface IErrorWithConfidence : IError
    {
        int ConfidenceMetric { get; }
    }
}