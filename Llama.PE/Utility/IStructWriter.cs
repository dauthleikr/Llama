using System;
using System.Collections.Generic;
using System.Text;

namespace Llama.PE.Utility
{
    internal interface IStructWriter
    {
        long Write<T>(T item, long rva = -1) where T : struct;
    }
}
