namespace Llama.PE.Packaging.PE32Plus.Reloc
{
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
            var rawData = new byte[param.Relocations.Size];
            var writer = new ArrayStructReaderWriter(rawData);
            _relocWriter.Write(writer, param.Relocations);

            Debug.Assert((int)writer.Offset == rawData.Length);
            return new RelocResult(rawData, (uint)rawData.Length, (uint)rawData.Length);
        }
    }
}