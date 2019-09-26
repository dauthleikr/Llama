namespace Llama.Compiler
{
    using System;

    internal class FunctionContext : IFunctionContext
    {
        private readonly int _calleeParameterSpace;
        private readonly string[] _locals;

        public FunctionContext(string[] locals, int calleeParameterSpace)
        {
            _locals = locals;
            _calleeParameterSpace = calleeParameterSpace;
        }

        public int GetLocalOffset(string identifier) => Array.IndexOf(_locals, identifier) * 8 + _calleeParameterSpace;
    }
}