namespace Llama.BinaryUtils
{
    public interface IStructWriter
    {
        long RVA { get; set; }
        long Write<T>(T item, long rva = -1) where T : struct;
    }
}