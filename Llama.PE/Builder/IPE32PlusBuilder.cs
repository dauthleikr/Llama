namespace Llama.PE.Builder
{
    using System.Reflection.PortableExecutable;
    using System.Runtime.InteropServices;
    using Structures.Header;

    public interface IPE32PlusBuilder
    {
        Architecture Architecture { get; set; }
        Characteristics Characteristics { get; set; }
        Subsystem Subsystem { get; set; }
        DllCharacteristics DllCharacteristics { get; set; }

        IPE32PlusBuilder AddAdditionalSection(string name, uint size);

        IPE32PlusBuilder ImportFunction(string library, string function);

        IPE32PlusBuildResult Build(uint codeSectionSize);
    }
}