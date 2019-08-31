namespace Llama.PE.Builder
{
    using System;
    using System.Collections.Generic;
    using System.Reflection.PortableExecutable;
    using System.Runtime.InteropServices;
    using System.Text;

    public class PE32PlusBuilder : IPE32PlusBuilder
    {
        public Architecture Architecture { get; set; } = Architecture.X64;

        // ReSharper disable once BitwiseOperatorOnEnumWithoutFlags
        public Characteristics Characteristics { get; set; } = Characteristics.ExecutableImage | Characteristics.LargeAddressAware;
        public Subsystem Subsystem { get; set; } = Subsystem.WindowsCui;
        public DllCharacteristics DllCharacteristics { get; set; }

        private readonly HashSet<(string name, uint size)> _additionalSections = new HashSet<(string name, uint size)>();
        private readonly HashSet<(string library, string function)> _imports = new HashSet<(string library, string function)>();

        public IPE32PlusBuilder AddAdditionalSection(string name, uint size)
        {
            if (string.IsNullOrWhiteSpace(name))
                throw new ArgumentException("Value cannot be null or whitespace.", nameof(name));
            if (Encoding.ASCII.GetBytes(name).Length > 8)
                throw new ArgumentException("Name cannot be longer than 8 bytes", nameof(name));

            _additionalSections.Add((name, size));
            return this;
        }

        public IPE32PlusBuildResult AddRelocation64(string section, uint sectionOffset) => throw new NotImplementedException();

        public IPE32PlusBuilder ImportFunction(string library, string function)
        {
            if (string.IsNullOrWhiteSpace(library))
                throw new ArgumentException("Value cannot be null or whitespace.", nameof(library));
            if (string.IsNullOrWhiteSpace(function))
                throw new ArgumentException("Value cannot be null or whitespace.", nameof(function));

            _imports.Add((library, function));
            return this;
        }

        public IPE32PlusBuildResult Build(uint codeSectionSize) => throw new NotImplementedException();
    }
}