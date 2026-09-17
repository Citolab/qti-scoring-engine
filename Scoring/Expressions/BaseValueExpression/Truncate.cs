using Citolab.QTI.ScoringEngine.Expressions.ConditionExpressions;
using Citolab.QTI.ScoringEngine.Helpers;
using Citolab.QTI.ScoringEngine.Interfaces;
using Citolab.QTI.ScoringEngine.Model;
using System;

namespace Citolab.QTI.ScoringEngine.Expressions.BaseValueExpression
{
    /// <summary>
    /// qti-truncate: the integer part of a float, towards zero.
    /// </summary>
    internal class Truncate : ValueExpressionBase
    {
        public override BaseValue Apply(IProcessingContext ctx)
        {
            if (expressions.Count != 1)
            {
                ctx.LogError($"qti-truncate should have exactly one child expression, found: {expressions.Count}");
                return null;
            }
            var baseValue = expressions[0].Apply(ctx);
            if (!baseValue.TryGetNumber(out var value))
            {
                ctx.LogInformation($"qti-truncate: '{baseValue?.Value}' is not a number, returning null.");
                return null;
            }
            return ((int)Math.Truncate(value)).ToBaseValue();
        }
    }
}
