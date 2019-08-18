namespace Llama.BinaryUtils
{
    public interface IStructReader
    {
        long RVA { get; set; }
        T Read<T>(long rva = -1) where T : struct;
    }
}