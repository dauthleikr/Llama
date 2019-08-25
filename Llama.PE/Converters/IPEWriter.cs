namespace Llama.PE
{
    using BinaryUtils;

    public interface IPEWriter<in T>
    {
        void Write(IStructWriter writer, T item);
    }
}