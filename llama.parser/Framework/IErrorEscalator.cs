namespace Llama.Parser.Framework
{
    using Abstractions;

    public interface IErrorEscalator
    {
        void Escalate(IErrorWithConfidence error);
    }
}