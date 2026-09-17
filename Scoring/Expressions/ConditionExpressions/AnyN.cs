using Citolab.QTI.ScoringEngine.Helpers;
using Citolab.QTI.ScoringEngine.Interfaces;
using System.Linq;

namespace Citolab.QTI.ScoringEngine.Expressions.ConditionExpressions
{
    /// <summary>
    /// qti-any-n: true when the number of children that are true is between min and max.
    /// </summary>
    internal class AnyN : ConditionExpressionBase
    {
        public override bool Execute(IProcessingContext ctx)
        {
            var minAttribute = GetAttributeValue("min");
            var maxAttribute = GetAttributeValue("max");
            if (!Helper.TryResolveInteger(minAttribute, ctx, out var min))
            {
                ctx.LogError($"qti-any-n has a missing or invalid min attribute: '{minAttribute}'");
                return false;
            }
            if (!Helper.TryResolveInteger(maxAttribute, ctx, out var max))
            {
                ctx.LogError($"qti-any-n has a missing or invalid max attribute: '{maxAttribute}'");
                return false;
            }
            var matches = conditionalExpressions.Count(expression => expression.Execute(ctx));
            ctx.LogInformation($"qti-any-n: {matches} of {conditionalExpressions.Count} children are true, expected between {min} and {max}.");
            return matches >= min && matches <= max;
        }
    }
}
