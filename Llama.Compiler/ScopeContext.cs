namespace Llama.Compiler
{
    using System;
    using System.Diagnostics;
    using System.Linq;

    public class ScopeContext : IScopeContext
    {
        public int TotalFunctionStackSpace { get; }
        private readonly int _calleeParameterSpace;
        private readonly string[] _locals;

        public ScopeContext(string[] locals, int calleeParameterSpace, int totalFunctionStackSpace)
        {
            if (locals == null)
                throw new ArgumentNullException(nameof(locals));

            Debug.Assert(totalFunctionStackSpace >= locals.Length * 8 + calleeParameterSpace);
            _locals = locals;
            _calleeParameterSpace = calleeParameterSpace;
            TotalFunctionStackSpace = totalFunctionStackSpace;
        }

        public int GetLocalOffset(string identifier)
        {
            var index = Array.IndexOf(_locals, identifier);
            if (index < 0)
                return index;
            return index * 8 + _calleeParameterSpace;
        }

        public bool HasLocal(string identifier) => _locals.Contains(identifier);
    }
}