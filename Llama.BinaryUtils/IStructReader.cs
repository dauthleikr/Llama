namespace Llama.BinaryUtils
{
    public interface IStructReader
    {
        long RVA { get; }
        T Read<T>(long rva = -1) where T : struct;
    }
}