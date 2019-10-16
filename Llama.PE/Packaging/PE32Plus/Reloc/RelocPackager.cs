namespace Llama.PE.Packaging.PE32Plus.Reloc
{
    using System.Collections.Generic;
    using System.Diagnostics;
    using BinaryUtils;
    using Converters;
    using Structures.Sections.Reloc;

    internal class RelocPackager : IPackage<IRelocInfo, IRelocResult>
    {
        private readonly IPEWriter<RelocationTable> _relocWriter;

        public RelocPackager(IPEWriter<RelocationTable> relocWriter) => _relocWriter = relocWriter;

        public IRelocResult Package(IRelocInfo param)
        {
            var relocationTable = param.Relocations;
            if (relocationTable.Size == 0)
            {
                var emptyBlock = new BaseRelocationBlockHeader { BlockSize = 8 };
                var emptyTable = new Dictionary<BaseRelocationBlockHeader, BaseRelocationBlockEntry[]>();
                emptyTable[emptyBlock] = new BaseRelocationBlockEntry[0];
                relocationTable = new RelocationTable(emptyTable);
            }

            var rawData = new byte[relocationTable.Size];
            var writer = new ArrayStructReaderWriter(rawData);

            _relocWriter.Write(writer, relocationTable);

            Debug.Assert((int)writer.Offset == rawData.Length);
            return new RelocResult(rawData, (uint)rawData.Length, (uint)rawData.Length);
        }
    }
}