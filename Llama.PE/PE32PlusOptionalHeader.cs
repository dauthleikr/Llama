namespace Llama.PE
{
    using System.Runtime.InteropServices;

    /// <summary>
    /// Comes right after the <see cref="PEHeader"/>. Is "optional" in the sense that for example object files do not have it. It is not optional for executeable Image files.
    /// Consists of the Standard, the Windows-specific and the data directories part.
    /// </summary>
    [StructLayout(LayoutKind.Sequential, CharSet = CharSet.Ansi, Pack = 1)]
    public struct PE32PlusOptionalHeader
    {
        public PE32PlusOptionalHeaderStandard Standard;
        public PE32PlusOptionalHeaderWinNT WinNT;
    }
}