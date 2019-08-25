using System;
using System.Collections.Generic;
using System.Text;

namespace Llama.PE.Builder
{
    using System.Reflection.PortableExecutable;
    using System.Runtime.InteropServices;

    interface IPE32PlusBuilder
    {
        IPE32PlusBuilder SetArchitecture(Architecture architecture);

        IPE32PlusBuilder SetCharacteristics(Characteristics characteristics);

        IPE32PlusBuilder SetSubsystem(Subsystem subsystem);

        IPE32PlusBuilder SetDllCharacteristics(DllCharacteristics characteristics);

        IPE32PlusBuilder AddAdditionalSection(string name, uint size);

        IPE32PlusBuilder ImportFunction(string library, string function);

        IPE32PlusBuildResult Build(uint codeSectionSize);
    }
}
