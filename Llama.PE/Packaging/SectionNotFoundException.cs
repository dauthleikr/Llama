namespace Llama.PE.Packaging.PE32Plus.Sections
{
    using System;

    internal class SectionNotFoundException : Exception
    {
        public SectionNotFoundException(string sectionName) : base($"Section {sectionName} does not exist") { }
    }
}