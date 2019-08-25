using System;
using System.Collections.Generic;
using System.Text;

namespace Llama.PE.Packaging.PE32Plus.Idata
{
    internal interface IIdataInfo
    {
        IEnumerable<(string library, string function)> Imports { get; }
        uint IdataRVA { get; }
        uint IATBlockSize { get; }
    }
}
