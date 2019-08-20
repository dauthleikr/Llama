namespace Llama.PE
{
    using BinaryUtils;

    public class GenericStructMapper<T> : IPEMapper<T> where T : struct
    {
        public void Write(T representation, IStructWriter writer)
        {
            writer.Write(representation);
        }

        public T Read(IStructReader reader) => reader.Read<T>();
    }
}