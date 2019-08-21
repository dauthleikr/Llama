using System;
using System.Collections.Generic;
using System.Text;

namespace Llama.PE
{
    using Header;

    public interface IPE32PlusContext
    {
        MZHeader MZHeader { get; }
        PEHeader PEHeader { get; }
        PE32PlusOptionalHeader OptionalHeader { get; }
        SectionHeader[] SectionHeaders { get; }

        ulong GetFileOffset(ulong rva);
    }
}
