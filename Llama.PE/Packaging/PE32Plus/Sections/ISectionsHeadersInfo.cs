namespace Llama.PE.Packaging.PE32Plus.Sections
{
    using System.Collections.Generic;
    using Idata;

    internal interface ISectionHeadersInfo
    {
        IEnumerable<ISectionInfo> OtherSections { get; }
        ICodeInfo TextSection { get; }
        IIdataResult IdataSection { get; }
        uint FileAlignment { get; }
        uint SectionAlignment { get; }
        uint FileOffsetAtSectionsHeader { get; }
    }
}