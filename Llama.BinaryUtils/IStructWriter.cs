namespace Llama.BinaryUtils
{
    public interface IStructWriter : IHaveRVA
    {
        ulong Write<T>(T item) where T : struct;
    }
}