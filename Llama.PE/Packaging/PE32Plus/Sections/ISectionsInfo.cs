namespace Llama.PE.Packaging.PE32Plus.Sections
{
    using System.Collections.Generic;
    using Idata;

    internal interface ISectionsInfo
    {
        IEnumerable<ISectionInfo> OtherSections { get; }
        ICodeInfo TextSection { get; }
        IEnumerable<(string library, string function)> Imports { get; }
        uint FileAlignment { get; }
        uint SectionAlignment { get; }
        uint FileOffsetAtSectionsHeader { get; }
    }
}