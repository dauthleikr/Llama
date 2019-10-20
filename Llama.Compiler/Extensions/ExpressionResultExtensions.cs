namespace Llama.Compiler.Extensions
{
    using System;
    using spit;

    internal static class ExpressionResultExtensions
    {
        public static void XorTo(this ExpressionResult source, Register target, CodeGen codeGen, IAddressFixer fixer)
        {
            source.DereferenceToRegister(
                target,
                codeGen,
                fixer,
                codeGen.Xor,
                codeGen.XorFromDereferenced,
                codeGen.XorFromDereferenced2,
                codeGen.XorFromDereferenced3,
                codeGen.XorFromDereferenced4
            );
        }

        public static void TestTo(this ExpressionResult source, Register other, CodeGen codeGen, IAddressFixer fixer)
        {
            source.DereferenceFromRegister(
                codeGen,
                other,
                fixer,
                codeGen.Test,
                codeGen.TestToDereferenced,
                codeGen.TestToDereferenced2,
                codeGen.TestToDereferenced3,
                codeGen.TestToDereferenced4
            );
        }

        public static void CmpTo(this ExpressionResult source, Register target, CodeGen codeGen, IAddressFixer fixer)
        {
            source.DereferenceToRegister(
                target,
                codeGen,
                fixer,
                codeGen.Cmp,
                codeGen.CmpFromDereferenced,
                codeGen.CmpFromDereferenced2,
                codeGen.CmpFromDereferenced3,
                codeGen.CmpFromDereferenced4
            );
        }

        public static void ComisdTo(this ExpressionResult source, Register target, CodeGen codeGen, IAddressFixer fixer)
        {
            source.DereferenceToRegister(
                target,
                codeGen,
                fixer,
                codeGen.Comisd,
                codeGen.ComisdFromDereferenced,
                codeGen.ComisdFromDereferenced2,
                codeGen.ComisdFromDereferenced3,
                codeGen.ComisdFromDereferenced4
            );
        }

        public static void ComissTo(this ExpressionResult source, Register target, CodeGen codeGen, IAddressFixer fixer)
        {
            source.DereferenceToRegister(
                target,
                codeGen,
                fixer,
                codeGen.Comiss,
                codeGen.ComissFromDereferenced,
                codeGen.ComissFromDereferenced2,
                codeGen.ComissFromDereferenced3,
                codeGen.ComissFromDereferenced4
            );
        }

        public static void ImulTo(this ExpressionResult source, Register target, CodeGen codeGen, IAddressFixer fixer)
        {
            if (target.BitSize == 8)
                throw new ArgumentException($"{nameof(ImulTo)} does not work with 8 bit types");

            source.DereferenceToRegister(
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

        public static void AddTo(this ExpressionResult source, Register target, CodeGen codeGen, IAddressFixer fixer)
        {
            source.DereferenceToRegister(
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

        public static void LeaTo(this ExpressionResult source, Register target, CodeGen codeGen, IAddressFixer fixer)
        {
            source.DereferenceToRegister(
                target,
                codeGen,
                fixer,
                codeGen.Mov,
                codeGen.LeaFromDereferenced,
                codeGen.LeaFromDereferenced2,
                codeGen.LeaFromDereferenced3,
                codeGen.LeaFromDereferenced4
            );
        }

        public static void MulsdTo(this ExpressionResult source, Register target, CodeGen codeGen, IAddressFixer fixer)
        {
            source.DereferenceToRegister(
                target,
                codeGen,
                fixer,
                (targetRegister, sourceRegister) => codeGen.Mulsd(targetRegister.AsFloat(), sourceRegister.AsFloat()),
                codeGen.MulsdFromDereferenced,
                codeGen.MulsdFromDereferenced2,
                codeGen.MulsdFromDereferenced3,
                codeGen.MulsdFromDereferenced4
            );
        }

        public static void MulssTo(this ExpressionResult source, Register target, CodeGen codeGen, IAddressFixer fixer)
        {
            source.DereferenceToRegister(
                target,
                codeGen,
                fixer,
                (targetRegister, sourceRegister) => codeGen.Mulss(targetRegister.AsFloat(), sourceRegister.AsFloat()),
                codeGen.MulssFromDereferenced,
                codeGen.MulssFromDereferenced2,
                codeGen.MulssFromDereferenced3,
                codeGen.MulssFromDereferenced4
            );
        }

        public static void DivsdTo(this ExpressionResult source, Register target, CodeGen codeGen, IAddressFixer fixer)
        {
            source.DereferenceToRegister(
                target,
                codeGen,
                fixer,
                (targetRegister, sourceRegister) => codeGen.Divsd(targetRegister.AsFloat(), sourceRegister.AsFloat()),
                codeGen.DivsdFromDereferenced,
                codeGen.DivsdFromDereferenced2,
                codeGen.DivsdFromDereferenced3,
                codeGen.DivsdFromDereferenced4
            );
        }

        public static void DivssTo(this ExpressionResult source, Register target, CodeGen codeGen, IAddressFixer fixer)
        {
            source.DereferenceToRegister(
                target,
                codeGen,
                fixer,
                (targetRegister, sourceRegister) => codeGen.Divss(targetRegister.AsFloat(), sourceRegister.AsFloat()),
                codeGen.DivssFromDereferenced,
                codeGen.DivssFromDereferenced2,
                codeGen.DivssFromDereferenced3,
                codeGen.DivssFromDereferenced4
            );
        }

        public static void AddssTo(this ExpressionResult source, Register target, CodeGen codeGen, IAddressFixer fixer)
        {
            source.DereferenceToRegister(
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
            source.DereferenceToRegister(
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
            source.DereferenceToRegister(
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
            source.DereferenceToRegister(
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
            source.DereferenceToRegister(
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