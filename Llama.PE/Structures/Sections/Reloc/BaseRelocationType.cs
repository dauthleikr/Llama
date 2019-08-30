namespace Llama.PE.Structures.Sections.Reloc
{
    public enum BaseRelocationType
    {
        ImageRelBasedAbsolute = 0,
        ImageRelBasedHigh = 1,
        ImageRelBasedLow = 2,
        ImageRelBasedHighLow = 3,
        ImageRelBasedHighAdj = 4,
        ImageRelBasedMipsJmpaddr = 5,
        ImageRelBasedArmMov32 = 5,
        ImageRelBasedRiscvHigh20 = 5,
        Reserved = 6,
        ImageRelBasedThumbMov32 = 7,
        ImageRelBasedRiscvLow12I = 7,
        ImageRelBasedRiscvLow12S = 8,
        ImageRelBasedMipsJmpaddr16 = 9,
        ImageRelBasedDir64 = 10
    }
}