using System;
using System.Collections.Generic;
using System.Text;

namespace Llama.PE
{
    using System.Runtime.InteropServices;

    [StructLayout(LayoutKind.Sequential, CharSet = CharSet.Ansi, Pack = 1)]
    public struct ImageDataDirectory
    {
        public uint StartRVA;
        public uint Size;
    }
}
