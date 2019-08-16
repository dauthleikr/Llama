namespace Llama.PE.Utility
{
    internal interface IStructReader
    {
        T Read<T>(long rva = -1) where T : struct;
    }
}