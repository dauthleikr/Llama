namespace Llama.Parser.Tests.DebugImplementations
{
    using System.Diagnostics;
    using Abstractions;
    using Framework;

    internal class DebugErrorEscalator : IErrorEscalator
    {
        public void Escalate(IErrorWithConfidence error)
        {
            Debug.WriteLine($"ERROR: [{error.ConfidenceMetric}] {error.Index}-{error.Index + error.Length}: {error.Message}");
        }
    }
}