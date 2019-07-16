namespace Llama.Parser.Framework
{
    using System;
    using System.Diagnostics;
    using Abstractions;

    public class StandardErrorManager : IErrorManager
    {
        private readonly IErrorEscalator _errorEscalator;
        private readonly ErrorNode _errorRoot = new ErrorNode(null);
        private ErrorNode _errorCurrent;
        private int _level;

        public StandardErrorManager(IErrorEscalator escalator)
        {
            _errorEscalator = escalator;
            _errorCurrent = _errorRoot;
        }

        public void IncreaseLevel()
        {
            _level++;
        }

        public void Add(IError error, int confidenceMetric = 0)
        {
            Add(new ParseError(error, confidenceMetric));
        }

        public void Add(IErrorWithConfidence error)
        {
            if (_level < _errorCurrent.Depth)
            {
                Debug.Fail($"Bad level of {nameof(StandardErrorManager)}");
                _errorCurrent = _errorRoot;
            }

            if (_level > _errorCurrent.Depth)
                _errorCurrent = _errorRoot.AddChild(error);
            else if (_level == _errorCurrent.Depth)
                _errorCurrent.Parent.AddChild(error);
        }

        public void DecreaseLevel(bool success)
        {
            if (_level == 0)
                throw new InvalidOperationException("Error level already at 0, cannot decrease");

            if (success && _errorCurrent.Depth > _level)
            {
                _errorCurrent.Parent.KillChildren();
                _errorCurrent = _errorCurrent.Parent;
            }

            _level--;
        }

        public void EscalateAndClear()
        {
            _errorEscalator.Escalate(_errorRoot.GetHighest());
            _errorRoot.KillChildren();
        }
    }
}