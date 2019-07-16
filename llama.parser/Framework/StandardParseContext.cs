namespace Llama.Parser.Framework
{
    using System;
    using System.Collections.Generic;
    using System.Diagnostics;

    public class StandardParseContext : IParseContext
    {
        private readonly List<IStandardParseContextDebugHook> _debugHooks = new List<IStandardParseContextDebugHook>();
        private readonly IErrorManager _errorManager;
        private readonly INonCodeParser _nonCodeParser;
        private readonly IPanicResolverStrategies _panicResolvers;
        private readonly IParseStrategies _parsers;
        private readonly ISourceReader _reader;

        public StandardParseContext(ISourceReader reader, IErrorManager errorManager, IPanicResolverStrategies panicResolvers, IParseStrategies parsers, INonCodeParser nonCodeParser)
        {
            _reader = reader;
            _errorManager = errorManager;
            _panicResolvers = panicResolvers;
            _parsers = parsers;
            _nonCodeParser = nonCodeParser;
        }

        public bool TryReadToken<T>(out T result) where T : class, IToken
        {
            var token = TryReadToken<T>();
            result = token.ResultSuccess;
            return token.Successful;
        }

        public ITokenizationResult<T> TryReadToken<T>() where T : class, IToken
        {
            var strategy = _parsers.GetStrategyFor<T>();
            if (strategy == null)
                throw new InvalidOperationException($"Cannot find strategy to parse {typeof(T).Name}");

            if (!strategy.IsPlausible(_reader, this))
            {
                CallIncreaseLevelDebugHooks(typeof(T));
                CallNotPlausibleDebugHooks();
                CallDecreaseLevelDebugHooks();
                return TokenizationResult<T>.ErrorNotPlausible;
            }

            var strategySuccess = false;
            _errorManager.IncreaseLevel();
            CallIncreaseLevelDebugHooks(typeof(T));

            try
            {
                var startPosition = _reader.Position;
                CallParsingStartDebugHooks();
                var strategyResult = strategy.TryReadToken(_reader, this, _nonCodeParser);
                CallParsingEndDebugHooks(strategyResult);
                if (!(strategySuccess = strategyResult.Successful))
                {
                    _reader.Position = startPosition;
                    _errorManager.Add(strategyResult.ResultError);
                }

                return strategyResult;
            }
            finally
            {
                _errorManager.DecreaseLevel(strategySuccess);
                CallDecreaseLevelDebugHooks();
            }
        }

        public bool IsPlausible<T>() where T : class, IToken => _parsers.GetStrategyFor<T>().IsPlausible(_reader, this);

        public void Panic<T>() where T : IPanicResolver
        {
            var faultyCodeLength = _panicResolvers.GetStrategy<T>().GetFaultyCodeLength(_reader);
            _nonCodeParser.MarkAsNonCode(_reader.Position, faultyCodeLength);
            _errorManager.EscalateAndClear();
        }

        public void AddDebugHook(IStandardParseContextDebugHook debugHook) => _debugHooks.Add(debugHook);

        public bool RemoveDebugHook(IStandardParseContextDebugHook debugHook) => _debugHooks.Remove(debugHook);

        [Conditional("DEBUG")]
        private void CallIncreaseLevelDebugHooks(Type tokenType)
        {
            foreach (var hook in _debugHooks)
                hook.IncreaseLevel(tokenType);
        }

        [Conditional("DEBUG")]
        private void CallDecreaseLevelDebugHooks()
        {
            foreach (var hook in _debugHooks)
                hook.DecreaseLevel();
        }

        [Conditional("DEBUG")]
        private void CallNotPlausibleDebugHooks()
        {
            foreach (var hook in _debugHooks)
                hook.ParsingSkippedNotPlausible(_reader);
        }

        [Conditional("DEBUG")]
        private void CallParsingEndDebugHooks<T>(ITokenizationResult<T> result) where T : class, IToken
        {
            foreach (var hook in _debugHooks)
                hook.ParsingEnd(result);
        }


        [Conditional("DEBUG")]
        private void CallParsingStartDebugHooks()
        {
            foreach (var hook in _debugHooks)
                hook.ParsingStart(_reader);
        }
    }
}