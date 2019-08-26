namespace Llama.PE.Packaging.PE32Plus.Idata
{
    using System.Collections.Generic;
    using System.Diagnostics;
    using System.IO;
    using System.Linq;
    using System.Text;
    using BinaryUtils;
    using Converters;
    using Structures.Sections.Idata;

    internal class IdataSectionPackager : IPackage<IIdataInfo, IIdataResult>
    {
        private readonly IPEWriter<HintNameEntry> _hintNameConverter;

        public IdataSectionPackager(IPEWriter<HintNameEntry> hintNameConverter) => _hintNameConverter = hintNameConverter;

        public unsafe IIdataResult Package(IIdataInfo param)
        {
            var imports = param.Imports.GroupBy(item => item.library).ToArray();

            var iatSize = imports.Sum(group => Round.Up(group.Count() * sizeof(ImportLookupEntryPE32Plus), param.IATBlockSize));
            var directoryTableSize = (imports.Length + 1) * sizeof(ImportDirectoryEntry);
            var lookupTableSize = imports.Sum(group => (group.Count() + 1) * sizeof(ImportLookupEntryPE32Plus));

            var iatOffset = 0u;
            var directoryTableOffset = (uint)iatSize;
            var lookupTableOffset = (uint)(directoryTableOffset + directoryTableSize);
            var hintNameOffset = (uint)(lookupTableOffset + lookupTableSize);

            var dataStream = new MemoryStream();
            var structWriter = new StreamStructReaderWriter(dataStream);
            var libAndFuncToRVAOfIATEntry = new Dictionary<(string lib, string func), uint>();

            foreach (var import in imports)
            {
                WriteAndIncreaseOffset(
                    structWriter,
                    new ImportDirectoryEntry
                    {
                        IAT_RVA = iatOffset + param.IdataRVA,
                        ImportLookupTableRVA = lookupTableOffset + param.IdataRVA,
                        NameRVA = hintNameOffset + param.IdataRVA
                    },
                    ref directoryTableOffset
                );
                WriteAndIncreaseOffset(structWriter, Encoding.ASCII.GetBytes(import.Key + "\0"), ref hintNameOffset);

                foreach (var (libName, functionName) in import)
                {
                    var lookupEntry = new ImportLookupEntryPE32Plus { HintNameTableRVA = hintNameOffset + param.IdataRVA };
                    libAndFuncToRVAOfIATEntry[(libName, functionName)] = iatOffset + param.IdataRVA;
                    WriteAndIncreaseOffset(structWriter, lookupEntry, ref iatOffset);
                    WriteAndIncreaseOffset(structWriter, lookupEntry, ref lookupTableOffset);
                    WriteAndIncreaseOffset(structWriter, new HintNameEntry(0 /*todo?*/, functionName), ref hintNameOffset);
                }

                iatOffset = Round.Up(iatOffset, param.IATBlockSize);
                WriteAndIncreaseOffset(structWriter, default(ImportLookupEntryPE32Plus), ref lookupTableOffset);
            }

            Debug.Assert(iatOffset <= iatSize);
            Debug.Assert(directoryTableOffset <= iatSize + directoryTableSize);
            Debug.Assert(lookupTableOffset <= iatSize + directoryTableSize + lookupTableSize);

            return new IdataResult(param.IdataRVA, param.IdataRVA + (uint)iatSize, param.IdataRVA, dataStream.ToArray(), libAndFuncToRVAOfIATEntry);
        }

        private void WriteAndIncreaseOffset<T>(IStructWriter writer, T item, ref uint offset) where T : struct
        {
            writer.Write(item, offset);
            offset = (uint)writer.Offset;
        }

        private void WriteAndIncreaseOffset<T>(IStructWriter writer, T[] items, ref uint offset) where T : struct
        {
            writer.WriteArray(items, offset);
            offset = (uint)writer.Offset;
        }

        private void WriteAndIncreaseOffset(IStructWriter writer, HintNameEntry item, ref uint offset)
        {
            writer.Offset = offset;
            _hintNameConverter.Write(writer, item);
            offset = (uint)writer.Offset;
        }
    }
}