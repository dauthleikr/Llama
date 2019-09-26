namespace Llama.Compiler
{
    using System;
    using System.Diagnostics;

    internal class FunctionContext : IFunctionContext
    {
        private readonly int _calleeParameterSpace;
        private readonly string[] _locals;

        public FunctionContext(string[] locals, int calleeParameterSpace, int totalFunctionStackSpace)
        {
            if (locals == null)
                throw new ArgumentNullException(nameof(locals));

            Debug.Assert(totalFunctionStackSpace >= locals.Length * 8 + calleeParameterSpace);
            _locals = locals;
            _calleeParameterSpace = calleeParameterSpace;
            TotalFunctionStackSpace = totalFunctionStackSpace;
        }

        public int TotalFunctionStackSpace { get; }
        public int GetLocalOffset(string identifier) => Array.IndexOf(_locals, identifier) * 8 + _calleeParameterSpace;
    }
}