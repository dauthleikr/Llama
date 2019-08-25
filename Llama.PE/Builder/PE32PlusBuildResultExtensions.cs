using System;
using System.Collections.Generic;
using System.Text;

namespace Llama.PE.Builder
{
    public static class PE32PlusBuildResultExtensions
    {
        public static byte[] GetCodeSectionBuffer(this IPE32PlusBuildResult result) => result.GetSectionBuffer(".text");
    }
}
