namespace Llama.PE.Builder.PE32Plus
{
    using System;
    using System.IO;
    using System.Linq;
    using Packaging.PE32Plus.Executable;
    using Packaging.PE32Plus.Sections;

    internal class ExecutableBuilderResult : IExecutableBuilderResult
    {
        private readonly IExecutableResult _packagedExecutable;

        public ExecutableBuilderResult(IExecutableResult packagedExecutable) =>
            _packagedExecutable = packagedExecutable ?? throw new ArgumentNullException(nameof(packagedExecutable));

        public Span<byte> GetSectionBuffer(string sectionName)
        {
            if (sectionName == null)
                throw new ArgumentNullException(nameof(sectionName));

            var section = _packagedExecutable.SectionHeaders.FirstOrDefault(sec => sec.NameString.StartsWith(sectionName));
            if (section == default)
                throw new SectionNotFoundException(sectionName);

            return _packagedExecutable.RawData.AsSpan((int)section.PointerToRawData, (int)section.SizeOfRawData);
        }

        public long GetIATEntryOffsetToStartOfCode(string library, string function)
        {
            if (library == null)
                throw new ArgumentNullException(nameof(library));
            if (function == null)
                throw new ArgumentNullException(nameof(function));

            var rvaOfIATEntry = _packagedExecutable.IATResolver.GetRVAOfIATEntry(library, function);
            var codeSection = _packagedExecutable.SectionHeaders.First(sec => sec.NameString == ".text\0\0\0");
            return rvaOfIATEntry - (long)codeSection.VirtualAddress;
        }

        public long GetSectionOffsetFromStartOfCode(string sectionName)
        {
            if (sectionName == null)
                throw new ArgumentNullException(nameof(sectionName));

            var codeSection = _packagedExecutable.SectionHeaders.First(sec => sec.NameString == ".text\0\0\0");
            var otherSection = _packagedExecutable.SectionHeaders.FirstOrDefault(sec => sec.NameString.StartsWith(sectionName));
            if (otherSection == default)
                throw new SectionNotFoundException(sectionName);

            return otherSection.VirtualAddress - (long)codeSection.VirtualAddress;
        }

        public ulong GetSectionRVA(string sectionName)
        {
            if (sectionName == null)
                throw new ArgumentNullException(nameof(sectionName));

            var section = _packagedExecutable.SectionHeaders.FirstOrDefault(sec => sec.NameString.StartsWith(sectionName));
            if (section == default)
                throw new SectionNotFoundException(sectionName);

            return section.VirtualAddress;
        }

        public void Finish(Stream output)
        {
            if (output == null)
                throw new ArgumentNullException(nameof(output));

            output.Write(_packagedExecutable.RawData);
        }
    }
}