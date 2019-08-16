using System;
using System.Collections.Generic;
using System.Text;

namespace Llama.PE.idata
{
    using System.Runtime.InteropServices;

    [StructLayout(LayoutKind.Sequential, CharSet = CharSet.Ansi, Pack = 2)]
    public struct HintNameTable
    {
        public ushort ExportNamePointerTableIndex;
        [MarshalAs(UnmanagedType.LPStr)]
        public string Name;
    }
}
