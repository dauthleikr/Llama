namespace Llama.PE.Packaging.PE32Plus.Executable
{
    using System;
    using System.Collections.Generic;
    using Sections;

    internal class SectionHeadersInfo : ISectionHeadersInfo
    {
        public IEnumerable<ISectionInfo> OtherSections { get; }
        public ICodeInfo TextSection { get; }
        public IEnumerable<(string library, string function)> Imports { get; }
        public uint FileAlignment { get; }
        public uint SectionAlignment { get; }
        public uint FileOffsetAtSectionsHeader { get; }

        public SectionHeadersInfo(
            IEnumerable<ISectionInfo> otherSections,
            ICodeInfo textSection,
            IEnumerable<(string library, string function)> imports,
            uint fileAlignment,
            uint sectionAlignment,
            uint fileOffsetAtSectionsHeader
        )
        {
            OtherSections = otherSections ?? throw new ArgumentNullException(nameof(otherSections));
            TextSection = textSection ?? throw new ArgumentNullException(nameof(textSection));
            Imports = imports ?? throw new ArgumentNullException(nameof(imports));
            FileAlignment = fileAlignment;
            SectionAlignment = sectionAlignment;
            FileOffsetAtSectionsHeader = fileOffsetAtSectionsHeader;
        }
    }
}