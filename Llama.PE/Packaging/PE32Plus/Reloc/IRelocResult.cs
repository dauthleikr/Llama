using System;
using System.Collections.Generic;
using System.Text;

namespace Llama.PE.Packaging.PE32Plus.Reloc
{
    using Sections;

    interface IRelocResult : IPackagingResult, ISectionInfo
    {
    }
}
