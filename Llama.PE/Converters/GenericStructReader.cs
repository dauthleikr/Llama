namespace Llama.PE.Converters
{
    using BinaryUtils;

    public class GenericStructReader<T> : IPEReader<T> where T : struct
    {
        public T Read(IStructReader reader, IPE32PlusContext image) => reader.Read<T>();

        public void Write(T representation, IStructWriter writer)
        {
            writer.Write(representation);
        }
    }
}