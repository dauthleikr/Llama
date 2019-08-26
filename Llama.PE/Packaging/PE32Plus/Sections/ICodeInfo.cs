namespace Llama.PE.Packaging.PE32Plus.Sections
{
    internal interface ICodeInfo : ISectionInfo
    {
        uint EntryPointOffset { get; }
    }
}