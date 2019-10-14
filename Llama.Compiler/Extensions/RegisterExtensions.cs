namespace Llama.Compiler.Extensions
{
    using System;
    using System.Linq;
    using spit;

    public static class RegisterExtensions
    {
        private static readonly Register[] VolatileIntRegisters =
        {
            Register64.RAX,
            Register64.RCX,
            Register64.RDX,
            Register64.R8,
            Register64.R9,
            Register64.R10,
            Register64.R11,
        };

        private static readonly Register[] VolatileFloatRegisters =
        {
            XmmRegister.XMM0,
            XmmRegister.XMM1,
            XmmRegister.XMM2,
            XmmRegister.XMM3,
            XmmRegister.XMM4,
            XmmRegister.XMM5
        };

        public static Register OtherVolatileIntRegister(params Register[] occupiedRegisters) =>
            VolatileIntRegisters.FirstOrDefault(vol => occupiedRegisters.All(occ => !occ.IsSameRegister(vol)));
        public static Register OtherVolatileFloatRegister(params Register[] occupiedRegisters) =>
            VolatileFloatRegisters.FirstOrDefault(vol => occupiedRegisters.All(occ => !occ.IsSameRegister(vol)));

        private static void ChangeSizeSigned(this Register register, Register target, CodeGen gen)
        {
            switch (target.BitSize)
            {
                case 64:
                {
                    switch (register.BitSize)
                    {
                        case 8:
                            gen.Movsx(target.AsR64(), target.AsR8());
                            break;
                        case 16:
                            gen.Movsx(target.AsR64(), target.AsR16());
                            break;
                        case 32:
                            gen.Movsxd(target.AsR64(), target.AsR32());
                            break;
                        default:
                            throw new NotImplementedException();
                    }
                }
                    break;
                case 32:
                {
                    switch (register.BitSize)
                    {
                        case 8:
                            gen.Movsx(target.AsR32(), target.AsR8());
                            break;
                        case 16:
                            gen.Movsx(target.AsR32(), target.AsR16());
                            break;
                        default:
                            throw new NotImplementedException();
                    }
                }
                    break;
                case 16:
                    if (register.BitSize != 8)
                        throw new NotImplementedException();
                    gen.Movsx(target.AsR16(), register.AsR8());
                    break;
                default:
                    throw new NotImplementedException();
            }
        }

        private static void ChangeSizeUnsigned(this Register register, Register target, CodeGen gen)
        {
            switch (target.BitSize)
            {
                case 64:
                {
                    switch (register.BitSize)
                    {
                        case 8:
                            gen.Movzx(target.AsR64(), target.AsR8());
                            break;
                        case 16:
                            gen.Movzx(target.AsR64(), target.AsR16());
                            break;
                        case 32:
                            gen.Mov(target.AsR64(), target.AsR32());
                            break;
                        default:
                            throw new NotImplementedException();
                    }
                }
                    break;
                case 32:
                {
                    switch (register.BitSize)
                    {
                        case 8:
                            gen.Movzx(target.AsR32(), target.AsR8());
                            break;
                        case 16:
                            gen.Movzx(target.AsR32(), target.AsR16());
                            break;
                        default:
                            throw new NotImplementedException();
                    }
                }
                    break;
                case 16:
                    if (register.BitSize != 8)
                        throw new NotImplementedException();
                    gen.Movzx(target.AsR16(), register.AsR8());
                    break;
                default:
                    throw new NotImplementedException();
            }
        }

        public static void ChangeSize(this Register register, Register target, CodeGen gen, bool unsigned)
        {
            if (register.BitSize >= target.BitSize)
                return;

            if (unsigned)
                ChangeSizeUnsigned(register, target, gen);
            else
                ChangeSizeSigned(register, target, gen);
        }
    }
}