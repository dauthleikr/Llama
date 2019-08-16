namespace Llama.BinaryUtils
{
    public interface IStructReader
    {
        T Read<T>(long rva = -1) where T : struct;
    }
}