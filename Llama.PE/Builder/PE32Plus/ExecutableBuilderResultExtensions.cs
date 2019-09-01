namespace Llama.PE.Builder.PE32Plus
{
    using System;

    public static class PE32PlusBuildResultExtensions
    {
        public static Span<byte> GetCodeSectionBuffer(this IExecutableBuilderResult result) => result.GetSectionBuffer(".text");
    }
}