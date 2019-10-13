namespace Llama.Compiler
{
    using System.Collections.Generic;
    using System.Linq;
    using BinaryUtils;
    using spit;

    public class StorageManager
    {
        private readonly HashSet<Register> _borrowedRegisters = new HashSet<Register>();
        private int _stackNeededForFunction;
        private readonly Stack<Storage> _registerStorageFloat = new Stack<Storage>();
        private readonly Stack<Storage> _registerStorageInt = new Stack<Storage>();

        public StorageManager(IScopeContext functionScope)
        {
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

            var storage = new Storage(_stackNeededForFunction, true);
            _stackNeededForFunction += 8;
            return storage;
        }

        private Storage GetOrMakeFloatStorage()
        {
            if (_registerStorageFloat.Count != 0)
                return _registerStorageFloat.Pop();

            var storage = new Storage(_stackNeededForFunction, false);
            _stackNeededForFunction += 8;
            return storage;
        }

        public void Release(Storage storage)
        {
            if (storage.IsIntegerType)
                _registerStorageInt.Push(storage);
            else
                _registerStorageFloat.Push(storage);
        }

        public void CreatePrologue(CodeGen codeGen)
        {
            foreach (var register in _borrowedRegisters.Where(reg => !reg.FloatingPoint).OrderBy(reg => reg.AsR64()))
                codeGen.Push(register.AsR64());

            //for now we assume llama only uses the scalar operating mode, thus we save and restore only the first 8 bytes
            var stackForFloatRegisters = _borrowedRegisters.Count(reg => reg.FloatingPoint) * 8;
            var totalStack = stackForFloatRegisters + _stackNeededForFunction;
            while (totalStack % 16 != 8) // make aligned by 8 but not by 16 (call will align)
                totalStack = Round.AlwaysUp(totalStack, 8);
            if (totalStack == 0)
                return;
            if (totalStack <= sbyte.MaxValue)
                codeGen.Sub(Register64.RSP, (sbyte)totalStack);
            else
                codeGen.Sub(Register64.RSP, totalStack);

            foreach (var register in _borrowedRegisters.Where(reg => reg.FloatingPoint).OrderBy(reg => reg.AsFloat()))
                codeGen.MovqToDereferenced(Register64.RSP, register.AsFloat(), totalStack -= 8);
        }

        public void CreateEpilogue(CodeGen codeGen)
        {
            //for now we assume llama only uses the scalar operating mode, thus we save and restore only the first 8 bytes
            var stackForFloatRegisters = _borrowedRegisters.Count(reg => reg.FloatingPoint) * 8;
            var totalStack = stackForFloatRegisters + _stackNeededForFunction;
            while (totalStack % 16 != 8) // make aligned by 8 but not by 16 (call will align)
                totalStack = Round.AlwaysUp(totalStack, 8);

            var totalStackCopy = totalStack;
            foreach (var register in _borrowedRegisters.Where(reg => reg.FloatingPoint).OrderByDescending(reg => reg.AsFloat()))
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