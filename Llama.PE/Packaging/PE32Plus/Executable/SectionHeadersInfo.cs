namespace Llama.PE.Packaging.PE32Plus.Executable
{
    using System;
    using System.Collections.Generic;
    using Sections;

    internal class SectionsInfo : ISectionsInfo
    {
        public IEnumerable<ISectionInfo> OtherSections { get; }
        public ICodeInfo TextSection { get; }
        public IEnumerable<(string library, string function)> Imports { get; }
        public IEnumerable<(string section, uint sectionOffset)> Relocations64 { get; }
        public uint FileAlignment { get; }
        public uint SectionAlignment { get; }
        public uint FileOffsetAtSectionsHeader { get; }

        public SectionsInfo(
            IEnumerable<ISectionInfo> otherSections,
            ICodeInfo textSection,
            IEnumerable<(string library, string function)> imports,
            uint fileAlignment,
            uint sectionAlignment,
            uint fileOffsetAtSectionsHeader,
            IEnumerable<(string section, uint sectionOffset)> relocations64
        )
        {
            OtherSections = otherSections ?? throw new ArgumentNullException(nameof(otherSections));
            TextSection = textSection ?? throw new ArgumentNullException(nameof(textSection));
            Imports = imports ?? throw new ArgumentNullException(nameof(imports));
            FileAlignment = fileAlignment;
            SectionAlignment = sectionAlignment;
            FileOffsetAtSectionsHeader = fileOffsetAtSectionsHeader;
            Relocations64 = relocations64 ?? throw new ArgumentNullException(nameof(relocations64));
        }
    }
}