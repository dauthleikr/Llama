namespace Llama.Compiler
{
    using spit;

    internal static class ExpressionResultExtensions
    {
        public static void AddTo(this ExpressionResult source, Register target, CodeGen codeGen, IAddressFixer fixer)
        {
            source.DereferenceTo(
                target,
                codeGen,
                fixer,
                codeGen.Add,
                codeGen.AddFromDereferenced,
                codeGen.AddFromDereferenced2,
                codeGen.AddFromDereferenced3,
                codeGen.AddFromDereferenced4
            );
        }

        public static void AddssTo(this ExpressionResult source, Register target, CodeGen codeGen, IAddressFixer fixer)
        {
            source.DereferenceTo(
                target,
                codeGen,
                fixer,
                (targetRegister, sourceRegister) => codeGen.Addss(targetRegister.AsFloat(), sourceRegister.AsFloat()),
                codeGen.AddssFromDereferenced,
                codeGen.AddssFromDereferenced2,
                codeGen.AddssFromDereferenced3,
                codeGen.AddssFromDereferenced4
            );
        }

        public static void AddsdTo(this ExpressionResult source, Register target, CodeGen codeGen, IAddressFixer fixer)
        {
            source.DereferenceTo(
                target,
                codeGen,
                fixer,
                (targetRegister, sourceRegister) => codeGen.Addsd(targetRegister.AsFloat(), sourceRegister.AsFloat()),
                codeGen.AddsdFromDereferenced,
                codeGen.AddsdFromDereferenced2,
                codeGen.AddsdFromDereferenced3,
                codeGen.AddsdFromDereferenced4
            );
        }

        public static void SubTo(this ExpressionResult source, Register target, CodeGen codeGen, IAddressFixer fixer)
        {
            source.DereferenceTo(
                target,
                codeGen,
                fixer,
                codeGen.Sub,
                codeGen.SubFromDereferenced,
                codeGen.SubFromDereferenced2,
                codeGen.SubFromDereferenced3,
                codeGen.SubFromDereferenced4
            );
        }

        public static void SubssTo(this ExpressionResult source, Register target, CodeGen codeGen, IAddressFixer fixer)
        {
            source.DereferenceTo(
                target,
                codeGen,
                fixer,
                (targetRegister, sourceRegister) => codeGen.Subss(targetRegister.AsFloat(), sourceRegister.AsFloat()),
                codeGen.SubssFromDereferenced,
                codeGen.SubssFromDereferenced2,
                codeGen.SubssFromDereferenced3,
                codeGen.SubssFromDereferenced4
            );
        }

        public static void SubsdTo(this ExpressionResult source, Register target, CodeGen codeGen, IAddressFixer fixer)
        {
            source.DereferenceTo(
                target,
                codeGen,
                fixer,
                (targetRegister, sourceRegister) => codeGen.Subsd(targetRegister.AsFloat(), sourceRegister.AsFloat()),
                codeGen.SubsdFromDereferenced,
                codeGen.SubsdFromDereferenced2,
                codeGen.SubsdFromDereferenced3,
                codeGen.SubsdFromDereferenced4
            );
        }
    }
}