namespace Llama.PE.Utility
{
    public interface IStructWriter
    {
        long Write<T>(T item, long rva = -1) where T : struct;
    }
}