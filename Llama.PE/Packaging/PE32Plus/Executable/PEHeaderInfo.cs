namespace Llama.PE.Packaging.PE32Plus.Executable
{
    using System;
    using System.Reflection.PortableExecutable;
    using PEHeader;

    internal class PEHeaderInfo : IPEInfo
    {
        public Characteristics Characteristics { get; }
        public Machine Architecture { get; }
        public DateTime TimeStamp { get; }
        public ushort NumberOfSections { get; }

        public PEHeaderInfo(Characteristics characteristics, Machine architecture, DateTime timeStamp, ushort numberOfSections)
        {
            Characteristics = characteristics;
            Architecture = architecture;
            TimeStamp = timeStamp;
            NumberOfSections = numberOfSections;
        }
    }
}