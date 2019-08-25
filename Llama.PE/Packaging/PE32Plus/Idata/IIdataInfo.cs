namespace Llama.PE.Packaging.PE32Plus.Idata
{
    using System.Collections.Generic;

    internal interface IIdataInfo
    {
        IEnumerable<(string library, string function)> Imports { get; }
        uint IdataRVA { get; }
        uint IATBlockSize { get; }
    }
}