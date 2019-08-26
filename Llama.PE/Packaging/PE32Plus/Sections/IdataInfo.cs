namespace Llama.PE.Packaging.PE32Plus.Sections
{
    using System.Collections.Generic;
    using Idata;

    internal class IdataInfo : IIdataInfo
    {
        public IEnumerable<(string library, string function)> Imports { get; }
        public uint IdataRVA { get; }
        public uint IATBlockSize { get; }

        public IdataInfo(IEnumerable<(string library, string function)> imports, uint idataRVA, uint iatBlockSize)
        {
            Imports = imports;
            IdataRVA = idataRVA;
            IATBlockSize = iatBlockSize;
        }
    }
}