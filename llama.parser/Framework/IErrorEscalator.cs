namespace Llama.Parser.Framework
{
    public interface IErrorEscalator
    {
        void Escalate(IErrorWithConfidence error);
    }
}