namespace Llama.PE.Packaging.PE32Plus.Section
{
    internal interface ISectionInfo
    {
        byte[] RawData { get; }
        string Name { get; }
    }
}