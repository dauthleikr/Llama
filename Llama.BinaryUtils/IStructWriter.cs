namespace Llama.BinaryUtils
{
    public interface IStructWriter : IHaveOffset
    {
        ulong Write<T>(T item) where T : struct;
    }
}