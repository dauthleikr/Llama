namespace Llama.Compiler
{
    using System.Collections.Generic;
    using System.Linq;
    using BinaryUtils;
    using Extensions;
    using Parser.Nodes;
    using spit;

    public class StorageManager
    {
        private readonly HashSet<Register> _borrowedRegisters = new HashSet<Register>();
        private readonly IScopeContext _functionScope;
        private readonly Stack<Storage> _registerStorageFloat = new Stack<Storage>();
        private readonly Stack<Storage> _registerStorageInt = new Stack<Storage>();
        private readonly Stack<Storage> _stackStorageFloat = new Stack<Storage>();
        private readonly Stack<Storage> _stackStorageInt = new Stack<Storage>();
        private int _stackNeededForFunction;

        public StorageManager(IScopeContext functionScope)
        {
            _functionScope = functionScope;
            _stackNeededForFunction = functionScope.TotalStackSpace;

            _registerStorageInt.Push(new Storage(Register64.R15));
            _registerStorageInt.Push(new Storage(Register64.R14));
            _registerStorageInt.Push(new Storage(Register64.R13));
            _registerStorageInt.Push(new Storage(Register64.R12));
            _registerStorageInt.Push(new Storage(Register64.RDI));
            _registerStorageInt.Push(new Storage(Register64.RSI));
            _registerStorageInt.Push(new Storage(Register64.RBX));

            _registerStorageFloat.Push(new Storage(XmmRegister.XMM15));
            _registerStorageFloat.Push(new Storage(XmmRegister.XMM14));
            _registerStorageFloat.Push(new Storage(XmmRegister.XMM13));
            _registerStorageFloat.Push(new Storage(XmmRegister.XMM12));
            _registerStorageFloat.Push(new Storage(XmmRegister.XMM11));
            _registerStorageFloat.Push(new Storage(XmmRegister.XMM10));
            _registerStorageFloat.Push(new Storage(XmmRegister.XMM9));
            _registerStorageFloat.Push(new Storage(XmmRegister.XMM8));
            _registerStorageFloat.Push(new Storage(XmmRegister.XMM7));
            _registerStorageFloat.Push(new Storage(XmmRegister.XMM6));
        }

        public Storage Allocate(bool integerType)
        {
            var storage = integerType ? GetOrMakeIntStorage() : GetOrMakeFloatStorage();
            if (storage.IsRegister)
                _borrowedRegisters.Add(storage.Register);
            return storage;
        }

        private Storage GetOrMakeIntStorage()
        {
            if (_registerStorageInt.Count != 0)
                return _registerStorageInt.Pop();

            if (_stackStorageInt.Count != 0)
                return _stackStorageInt.Pop();

            var storage = new Storage(_stackNeededForFunction, true);
            _stackNeededForFunction += 8;
            return storage;
        }

        private Storage GetOrMakeFloatStorage()
        {
            if (_registerStorageFloat.Count != 0)
                return _registerStorageFloat.Pop();

            if (_stackStorageFloat.Count != 0)
                return _stackStorageFloat.Pop();

            var storage = new Storage(_stackNeededForFunction, false);
            _stackNeededForFunction += 8;
            return storage;
        }

        public PreferredRegister MakePreferredRegister(Register64 fallbackInt = Register64.RAX, XmmRegister fallbackFloat = XmmRegister.XMM0) =>
            new PreferredRegister(
                _registerStorageInt.Count > 0 ? _registerStorageInt.Peek().Register.AsR64() : fallbackInt,
                _registerStorageFloat.Count > 0 ? _registerStorageFloat.Peek().Register.AsFloat() : fallbackFloat
            );

        public void Release(Storage storage)
        {
            if (storage.IsIntegerType)
            {
                if (storage.IsRegister)
                    _registerStorageInt.Push(storage);
                else
                    _stackStorageInt.Push(storage);
            }
            else
            {
                if (storage.IsRegister)
                    _registerStorageFloat.Push(storage);
                else
                    _stackStorageFloat.Push(storage);
            }
        }

        public void CreatePrologue(CodeGen codeGen, FunctionDeclaration function)
        {
            var borrowedIntRegisters = _borrowedRegisters.Where(reg => !reg.FloatingPoint).OrderBy(reg => reg.AsR64()).ToArray();
            foreach (var register in borrowedIntRegisters)
                codeGen.Push(register.AsR64());

            //for now we assume llama only uses the scalar operating mode, thus we save and restore only the first 8 bytes
            var stackForFloatRegisters = _borrowedRegisters.Count(reg => reg.FloatingPoint) * 8;
            var totalStack = stackForFloatRegisters + _stackNeededForFunction;
            while ((totalStack + borrowedIntRegisters.Length * 8) % 16 != 8) // make aligned by 8 but not by 16 (call will align)
                totalStack = Round.AlwaysUp(totalStack, 8);
            if (totalStack == 0)
                return;
            if (totalStack <= sbyte.MaxValue)
                codeGen.Sub(Register64.RSP, (sbyte)totalStack);
            else
                codeGen.Sub(Register64.RSP, totalStack);

            MoveParametersToLocals(
                codeGen,
                function,
                stackForFloatRegisters + 8
            ); // todo: rework this. If we know the borrowedRegisters beforehand, this does not need to be done _here_


            foreach (var register in _borrowedRegisters.Where(reg => reg.FloatingPoint).OrderBy(reg => reg.AsFloat()))
                codeGen.MovqToDereferenced(Register64.RSP, register.AsFloat(), totalStack -= 8);
        }

        private void MoveParametersToLocals(CodeGen codeGen, FunctionDeclaration function, int stackOffsetToParams)
        {
            static ExpressionResult MakeRegister(Type type, Register64 registerInt, XmmRegister registerFloat) =>
                new ExpressionResult(
                    type,
                    type.MakeRegisterWithCorrectSize(registerInt, registerFloat)
                );

            for (var index = 0; index < function.Parameters.Length; index++)
            {
                var parameter = function.Parameters[index];
                var target = _functionScope.GetLocalReference(parameter.ParameterIdentifier.RawText);
                var source = index switch
                {
                    0 => MakeRegister(parameter.ParameterType, Register64.RCX, XmmRegister.XMM0),
                    1 => MakeRegister(parameter.ParameterType, Register64.RDX, XmmRegister.XMM1),
                    2 => MakeRegister(parameter.ParameterType, Register64.R8, XmmRegister.XMM2),
                    3 => MakeRegister(parameter.ParameterType, Register64.R9, XmmRegister.XMM3),
                    _ => new ExpressionResult(
                        parameter.ParameterType,
                        Register64.RSP,
                        stackOffsetToParams + (index - 4) * 8,
                        Segment.SS
                    )
                };
                if (source.Kind == ExpressionResult.ResultKind.Value)
                    target.GenerateAssign(source.Value, codeGen, null);
                else
                {
                    var tempRegister = parameter.ParameterType.MakeRegisterWithCorrectSize(Register64.RAX, XmmRegister.XMM4);
                    source.GenerateMoveTo(tempRegister, codeGen, null);
                    target.GenerateAssign(tempRegister, codeGen, null);
                }
            }
        }

        public void CreateEpilogue(CodeGen codeGen)
        {
            var borrowedIntRegister = _borrowedRegisters.Where(reg => !reg.FloatingPoint).OrderBy(reg => reg.AsR64()).ToArray();

            //for now we assume llama only uses the scalar operating mode, thus we save and restore only the first 8 bytes
            var stackForFloatRegisters = _borrowedRegisters.Count(reg => reg.FloatingPoint) * 8;
            var totalStack = stackForFloatRegisters + _stackNeededForFunction;
            while ((totalStack + borrowedIntRegister.Length * 8) % 16 != 8) // make aligned by 8 but not by 16 (call will align)
                totalStack = Round.AlwaysUp(totalStack, 8);

            var totalStackCopy = totalStack;
            foreach (var register in _borrowedRegisters.Where(reg => reg.FloatingPoint).OrderBy(reg => reg.AsFloat()))
                codeGen.MovqToDereferenced(Register64.RSP, register.AsFloat(), totalStackCopy -= 8);


            if (totalStack <= sbyte.MaxValue)
                codeGen.Add(Register64.RSP, (sbyte)totalStack);
            else
                codeGen.Add(Register64.RSP, totalStack);

            foreach (var register in _borrowedRegisters.Where(reg => !reg.FloatingPoint).OrderByDescending(reg => reg.AsR64()))
                codeGen.Pop(register.AsR64());
        }
    }
}