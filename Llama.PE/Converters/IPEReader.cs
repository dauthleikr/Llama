namespace Llama.PE
{
    using BinaryUtils;

    public interface IPEReader<out T>
    {
        T Read(IStructReader reader, IPE32PlusContext image);
    }
}