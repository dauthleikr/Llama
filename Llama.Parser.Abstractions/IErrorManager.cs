namespace Llama.Parser.Abstractions
{
    public interface IErrorManager
    {
        void IncreaseLevel();
        void Add(IError error, int confidenceMetric = 0);
        void Add(IErrorWithConfidence error);
        void DecreaseLevel(bool success);
        void EscalateAndClear();
    }
}