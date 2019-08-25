namespace Llama.PE.Converters
{
    using BinaryUtils;

    public interface IPEReader<out T>
    {
        T Read(IStructReader reader, IPE32PlusContext image);
    }
}