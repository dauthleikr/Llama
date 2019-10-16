namespace Llama.Linker
{
    internal class FunctionFixup : IFixupInfo
    {
        public long Position { get; set; }
        public string Function { get; }

        public FunctionFixup(long position, string function)
        {
            Position = position;
            Function = function;
        }
    }
}