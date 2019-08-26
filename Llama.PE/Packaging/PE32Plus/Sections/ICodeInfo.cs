namespace Llama.PE.Packaging.PE32Plus.CodeSection
{
    using Section;

    internal interface ICodeInfo : ISectionInfo
    {
        uint EntryPointOffset { get; }
    }
}