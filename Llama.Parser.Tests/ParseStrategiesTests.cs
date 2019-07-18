namespace Llama.Parser.Tests
{
    using System.Diagnostics;
    using System.Linq;
    using Abstractions;
    using Framework;
    using NUnit.Framework;

    [TestFixture]
    internal class ParseStrategiesTests
    {
        [Test]
        public void AllParsersRegistered()
        {
            var strategies = new ParseStrategies();
            foreach (var type in typeof(Parsers.ExpressionParser).Assembly.GetTypes().Where(t => !t.IsAbstract && t.GetInterfaces().Any(i => i.IsGenericType && i.GetGenericTypeDefinition().IsAssignableFrom(typeof(IParse<>)))))
            {
                Assert.NotNull(strategies.GetType().GetMethod(nameof(strategies.GetStrategyFor)).MakeGenericMethod(type).Invoke(strategies, null), $"{type.Name} is not registered");
            }
        }
    }
}