namespace Llama.PE.Builder.PE32Plus
{
    using System.Reflection.PortableExecutable;

    public interface IExecutableBuilder
    {
        Subsystem Subsystem { get; set; }
        uint StackSizeReserve { get; set; }
        uint StackSizeCommit { get; set; }
        ulong ImageBase { get; set; }
        bool MayRunFromRemoveableDrive { get; }
        bool MayRunFromNetwork { get; }
        uint FileAlignment { get; }
        uint SectionAlignment { get; }
        ushort MajorImageVersion { get; }
        ushort MinorImageVersion { get; }

        IExecutableBuilder AddAdditionalSection(string name, uint size, SectionCharacteristics characteristics);

        IExecutableBuilder AddRelocation64(string section, uint sectionOffset);

        IExecutableBuilder ImportFunction(string library, string function);

        IExecutableBuilderResult Build(uint codeSectionSize, uint entryPointOffset);
    }
}