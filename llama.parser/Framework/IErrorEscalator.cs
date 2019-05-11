using System;
using System.Collections.Generic;
using System.Text;

namespace Llama.Parser.Framework
{
    public interface IErrorEscalator
    {
        void Escalate(IErrorWithConfidence error);
    }
}
