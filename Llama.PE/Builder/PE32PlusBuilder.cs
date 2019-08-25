namespace Llama.PE.Builder
{
    using System.Reflection.PortableExecutable;
    using System.Runtime.InteropServices;

    public class PE32PlusBuilder : IPE32PlusBuilder
    {
        public IPE32PlusBuilder SetArchitecture(Architecture architecture) => throw new System.NotImplementedException();

        public IPE32PlusBuilder SetCharacteristics(Characteristics characteristics) => throw new System.NotImplementedException();

        public IPE32PlusBuilder SetSubsystem(Subsystem subsystem) => throw new System.NotImplementedException();

        public IPE32PlusBuilder SetDllCharacteristics(DllCharacteristics characteristics) => throw new System.NotImplementedException();

        public IPE32PlusBuilder AddAdditionalSection(string name, uint size) => throw new System.NotImplementedException();

        public IPE32PlusBuilder ImportFunction(string library, string function) => throw new System.NotImplementedException();

        public IPE32PlusBuildResult Build(uint codeSectionSize) => throw new System.NotImplementedException();
    }
}