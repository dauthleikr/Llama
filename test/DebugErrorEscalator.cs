using System;
using System.Collections.Generic;
using System.Text;

namespace test
{
    using System.Diagnostics;
    using Llama.Parser.Framework;

    class DebugErrorEscalator : IErrorEscalator
    {
        public void Escalate(IErrorWithConfidence error)
        {
            Debug.WriteLine($"ERROR: [{error.ConfidenceMetric}] {error.Index}-{error.Index + error.Length}: {error.Message}");
        }
    }
}
