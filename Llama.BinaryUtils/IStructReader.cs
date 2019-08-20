namespace Llama.BinaryUtils
{
    public interface IStructReader : IHaveRVA
    {
        T Read<T>() where T : struct;
    }
}