namespace Llama.PE
{
    using BinaryUtils;

    /// <summary>
    /// Functionality for converting a class/struct into its desired binary representation (or the other way around)
    /// A custom implementation of this should be created if the binary representation can not be achieved using a struct with MarshalAs/...
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public interface IPEMapper<T>
    {
        void Write(T representation, IStructWriter writer);
        T Read(IStructReader reader);
    }
}