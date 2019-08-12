namespace Llama.PE
{
    using System.Runtime.InteropServices;

    [StructLayout(LayoutKind.Sequential, CharSet = CharSet.Ansi, Pack = 1)]
    public struct PEOptionalHeader
    {
        public ExecutableKind ExecutableKind;
    }
}