using Citolab.QTI.ScoringEngine.Expressions.ConditionExpressions;
using Citolab.QTI.ScoringEngine.Helpers;
using Citolab.QTI.ScoringEngine.Interfaces;
using Citolab.QTI.ScoringEngine.Model;
using System;
using System.Threading;

namespace Citolab.QTI.ScoringEngine.Expressions.BaseValueExpression
{
    /// <summary>
    /// qti-random: one randomly selected value from a container.
    /// </summary>
    internal class Random : ValueExpressionBase
    {
        // results can be processed in parallel and System.Random is not thread-safe, so every
        // thread gets its own, seeded so that two threads starting at once do not share a series.
        [ThreadStatic]
        private static System.Random _random;

        private static System.Random Randomizer
        {
            get
            {
                if (_random == null)
                {
                    _random = new System.Random(Environment.TickCount ^ (Thread.CurrentThread.ManagedThreadId * 31));
                }
                return _random;
            }
        }

        public override BaseValue Apply(IProcessingContext ctx)
        {
            if (expressions.Count != 1)
            {
                ctx.LogError($"qti-random should have exactly one child expression, found: {expressions.Count}");
                return null;
            }
            var container = expressions[0].Apply(ctx);
            var values = container.ToValueList();
            if (values == null || values.Count == 0)
            {
                ctx.LogInformation("qti-random on an empty container, returning null.");
                return null;
            }
            return new BaseValue
            {
                Identifier = container.Identifier,
                BaseType = container.BaseType,
                Cardinality = Cardinality.Single,
                Value = values[Randomizer.Next(values.Count)]
            };
        }
    }
}
