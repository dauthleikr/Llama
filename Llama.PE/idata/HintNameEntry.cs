﻿namespace Llama.PE.Idata
{
    public class HintNameEntry
    {
        public readonly ushort ExportNamePointerTableIndex;
        public readonly string Name;

        public HintNameEntry(ushort exportNamePointerTableIndex, string name)
        {
            ExportNamePointerTableIndex = exportNamePointerTableIndex;
            Name = name;
        }
    }
}