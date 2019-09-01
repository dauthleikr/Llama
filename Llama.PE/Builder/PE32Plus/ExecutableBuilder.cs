namespace Llama.PE.Builder.PE32Plus
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Reflection.PortableExecutable;
    using System.Runtime.InteropServices;
    using System.Text;
    using Converters;
    using Packaging.PE32Plus.Executable;
    using Packaging.PE32Plus.Idata;
    using Packaging.PE32Plus.MZHeader;
    using Packaging.PE32Plus.OptionalHeader;
    using Packaging.PE32Plus.PEHeader;
    using Packaging.PE32Plus.Reloc;
    using Packaging.PE32Plus.Sections;

    public class ExecutableBuilder : IExecutableBuilder
    {
        public Architecture Architecture { get; set; } = Architecture.X64;
        public Subsystem Subsystem { get; set; } = Subsystem.WindowsCui;
        public uint StackSizeReserve { get; set; } = 0x400_000;
        public uint StackSizeCommit { get; set; } = 0x4_000;
        public ulong ImageBase { get; set; } = 0x140_000_000;
        public bool MayRunFromRemoveableDrive { get; } = true;
        public bool MayRunFromNetwork { get; } = true;
        public uint FileAlignment { get; } = 0x200;
        public uint SectionAlignment { get; } = 0x1_000;
        public ushort MajorImageVersion { get; } = 0;
        public ushort MinorImageVersion { get; } = 0;

        private readonly List<(string name, uint size, SectionCharacteristics characteristics)> _additionalSections =
            new List<(string name, uint size, SectionCharacteristics characteristics)>();

        private readonly HashSet<(string library, string function)> _imports = new HashSet<(string library, string function)>();
        private readonly HashSet<(string section, uint sectionOffset)> _relocations = new HashSet<(string section, uint sectionOffset)>();

        public IExecutableBuilder AddAdditionalSection(string name, uint size, SectionCharacteristics characteristics)
        {
            if (string.IsNullOrWhiteSpace(name))
                throw new ArgumentException("Value cannot be null or whitespace.", nameof(name));
            if (Encoding.ASCII.GetBytes(name).Length > 8)
                throw new ArgumentException("Name cannot be longer than 8 bytes", nameof(name));

            _additionalSections.Add((name, size, characteristics));
            return this;
        }

        public IExecutableBuilder AddRelocation64(string section, uint sectionOffset)
        {
            if (string.IsNullOrWhiteSpace(section))
                throw new ArgumentException("Value cannot be null or whitespace.", nameof(section));

            _relocations.Add((section, sectionOffset));
            return this;
        }

        public IExecutableBuilder ImportFunction(string library, string function)
        {
            if (string.IsNullOrWhiteSpace(library))
                throw new ArgumentException("Value cannot be null or whitespace.", nameof(library));
            if (string.IsNullOrWhiteSpace(function))
                throw new ArgumentException("Value cannot be null or whitespace.", nameof(function));

            _imports.Add((library, function));
            return this;
        }

        public IExecutableBuilderResult Build(uint codeSectionSize, uint entryPointOffset)
        {
            var exeInfo = new ExecutableInfo(
                MajorImageVersion,
                MinorImageVersion,
                Subsystem,
                StackSizeReserve,
                StackSizeCommit,
                ImageBase,
                FileAlignment,
                SectionAlignment,
                MayRunFromRemoveableDrive,
                MayRunFromNetwork,
                _imports,
                _relocations,
                new CodeSectionInfo(new byte[codeSectionSize], codeSectionSize, entryPointOffset),
                _additionalSections.Select(sec => new SectionInfo(new byte[sec.size], sec.name, sec.size, sec.characteristics))
            );

            var packager = new ExecutablePackager(
                new MZHeaderPackager(),
                new PEHeaderPackager(),
                new OptionalHeaderPackager(),
                new SectionsPackager(new IdataSectionPackager(new HintNameEntryWriter()), new RelocPackager(new RelocationTableWriter()))
            );

            return new ExecutableBuilderResult(packager.Package(exeInfo));
        }
    }
}