namespace Llama.BinaryUtils
{
    public interface IStructReader : IHaveOffset
    {
        T Read<T>() where T : struct;
    }
}