namespace Llama.Compiler.Extensions
{
    using System;
    using spit;

    public static class CodeGenExtensions
    {
        public static void InsertCode(
            this CodeGen codeGen,
            ILinkingInfo myFixer,
            long position,
            ReadOnlySpan<byte> data,
            ILinkingInfo otherFixer = null
        )
        {
            myFixer.Insert(position, data.Length);
            otherFixer?.CopyTo(myFixer, position);
            codeGen.Insert(data, position);
        }

        public static void InsertCode(this CodeGen codeGen, ILinkingInfo myFixer, long position, Action<CodeGen> insertAction)
        {
            var insertCode = new CodeGen();
            insertAction(insertCode);
            myFixer.Insert(position, insertCode.GetBufferSpan().Length);
            codeGen.Insert(insertCode.GetBufferSpan(), position);
        }

        // todo: add seeking to codegen so that this can actually be an extension
        public static int InstructionLength(Action<CodeGen> genAction)
        {
            var gen = new CodeGen();
            genAction(gen);
            return (int)gen.StreamPosition;
        }
    }
}