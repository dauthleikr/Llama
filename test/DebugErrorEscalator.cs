namespace test
{
    using System.Diagnostics;
    using Llama.Parser.Abstractions;
    using Llama.Parser.Framework;

    internal class DebugErrorEscalator : IErrorEscalator
    {
        public void Escalate(IErrorWithConfidence error)
        {
            Debug.WriteLine($"ERROR: [{error.ConfidenceMetric}] {error.Index}-{error.Index + error.Length}: {error.Message}");
        }
    }
}