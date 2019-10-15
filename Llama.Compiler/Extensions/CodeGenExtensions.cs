namespace Llama.Compiler.Extensions
{
    using System;
    using spit;

    public static class CodeGenExtensions
    {
        public static void InsertCode(
            this CodeGen codeGen,
            IAddressFixer myFixer,
            long position,
            ReadOnlySpan<byte> data,
            IAddressFixer otherFixer = null
        )
        {
            myFixer.Insert(position, data.Length);
            otherFixer?.CopyTo(myFixer, position);
            codeGen.Insert(data, position);
        }

        public static void InsertCode(this CodeGen codeGen, IAddressFixer myFixer, long position, Action<CodeGen> insertAction)
        {
            var insertCode = new CodeGen();
            insertAction(insertCode);
            myFixer.Insert(position, insertCode.GetDataSpan().Length);
            codeGen.Insert(insertCode.GetDataSpan(), position);
        }
    }
}