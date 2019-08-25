namespace Llama.PE.Builder
{
    using System;

    public static class PE32PlusBuildResultExtensions
    {
        public static Span<byte> GetCodeSectionBuffer(this IPE32PlusBuildResult result) => result.GetSectionBuffer(".text");
    }
}