namespace Llama.PE.Builder
{
    using System.Reflection.PortableExecutable;
    using System.Runtime.InteropServices;

    public interface IPE32PlusBuilder
    {
        Architecture Architecture { get; set; }
        Subsystem Subsystem { get; set; }
        IPE32PlusBuilder AddAdditionalSection(string name, uint size);

        IPE32PlusBuilder AddRelocation64(string section, uint sectionOffset);

        IPE32PlusBuilder ImportFunction(string library, string function);

        IPE32PlusBuildResult Build(uint codeSectionSize);
    }
}