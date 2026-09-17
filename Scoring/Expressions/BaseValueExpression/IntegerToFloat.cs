using Citolab.QTI.ScoringEngine.Expressions.ConditionExpressions;
using Citolab.QTI.ScoringEngine.Helpers;
using Citolab.QTI.ScoringEngine.Interfaces;
using Citolab.QTI.ScoringEngine.Model;

namespace Citolab.QTI.ScoringEngine.Expressions.BaseValueExpression
{
    /// <summary>
    /// qti-integer-to-float: the same number, with base-type float.
    /// </summary>
    internal class IntegerToFloat : ValueExpressionBase
    {
        public override BaseValue Apply(IProcessingContext ctx)
        {
            if (expressions.Count != 1)
            {
                ctx.LogError($"qti-integer-to-float should have exactly one child expression, found: {expressions.Count}");
                return null;
            }
            var baseValue = expressions[0].Apply(ctx);
            if (!baseValue.TryGetNumber(out var value))
            {
                ctx.LogInformation($"qti-integer-to-float: '{baseValue?.Value}' is not a number, returning null.");
                return null;
            }
            return value.ToBaseValue();
        }
    }
}
