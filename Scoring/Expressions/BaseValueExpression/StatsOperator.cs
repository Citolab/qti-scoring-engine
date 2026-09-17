using Citolab.QTI.ScoringEngine.Expressions.ConditionExpressions;
using Citolab.QTI.ScoringEngine.Helpers;
using Citolab.QTI.ScoringEngine.Interfaces;
using Citolab.QTI.ScoringEngine.Model;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Citolab.QTI.ScoringEngine.Expressions.BaseValueExpression
{
    /// <summary>
    /// qti-stats-operator: a statistic over the numeric container of its single child.
    /// </summary>
    internal class StatsOperator : ValueExpressionBase
    {
        public override BaseValue Apply(IProcessingContext ctx)
        {
            var name = GetAttributeValue("name");
            if (expressions.Count != 1)
            {
                ctx.LogError($"qti-stats-operator should have exactly one child expression, found: {expressions.Count}");
                return null;
            }
            var container = expressions[0].Apply(ctx);
            var values = container.ToValueList();
            if (values == null || values.Count == 0)
            {
                ctx.LogInformation($"qti-stats-operator {name} on an empty container, returning null.");
                return null;
            }
            var numbers = new List<double>();
            foreach (var value in values)
            {
                if (!value.TryParseDouble(out var number))
                {
                    ctx.LogInformation($"qti-stats-operator {name}: '{value}' is not a number, returning null.");
                    return null;
                }
                numbers.Add(number);
            }
            var mean = numbers.Average();
            var sumOfSquares = numbers.Sum(n => (n - mean) * (n - mean));
            switch (name)
            {
                case "mean":
                    return mean.ToBaseValue();
                case "median":
                    {
                        var sorted = numbers.OrderBy(n => n).ToList();
                        var middle = sorted.Count / 2;
                        var median = sorted.Count % 2 == 1
                            ? sorted[middle]
                            : (sorted[middle - 1] + sorted[middle]) / 2.0;
                        return median.ToBaseValue();
                    }
                case "popVariance":
                    return (sumOfSquares / numbers.Count).ToBaseValue();
                case "popSD":
                    return Math.Sqrt(sumOfSquares / numbers.Count).ToBaseValue();
                // 'variance' is not a name of the spec, but it is used for the sample variance
                case "variance":
                case "sampleVariance":
                    {
                        if (numbers.Count < 2)
                        {
                            ctx.LogInformation($"qti-stats-operator {name} needs at least two values, returning null.");
                            return null;
                        }
                        return (sumOfSquares / (numbers.Count - 1)).ToBaseValue();
                    }
                case "sampleSD":
                    {
                        if (numbers.Count < 2)
                        {
                            ctx.LogInformation($"qti-stats-operator {name} needs at least two values, returning null.");
                            return null;
                        }
                        return Math.Sqrt(sumOfSquares / (numbers.Count - 1)).ToBaseValue();
                    }
                default:
                    {
                        ctx.LogError($"Unsupported name in qti-stats-operator: '{name}'");
                        return null;
                    }
            }
        }
    }
}
