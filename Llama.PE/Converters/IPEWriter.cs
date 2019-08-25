namespace Llama.PE.Converters
{
    using BinaryUtils;

    public interface IPEWriter<in T>
    {
        void Write(IStructWriter writer, T item);
    }
}