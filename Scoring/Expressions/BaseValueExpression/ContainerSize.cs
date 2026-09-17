using Citolab.QTI.ScoringEngine.Expressions.ConditionExpressions;
using Citolab.QTI.ScoringEngine.Helpers;
using Citolab.QTI.ScoringEngine.Interfaces;
using Citolab.QTI.ScoringEngine.Model;

namespace Citolab.QTI.ScoringEngine.Expressions.BaseValueExpression
{
    /// <summary>
    /// qti-container-size: the number of values in a container. NULL counts as empty, so this
    /// returns 0 rather than NULL.
    /// </summary>
    internal class ContainerSize : ValueExpressionBase
    {
        public override BaseValue Apply(IProcessingContext ctx)
        {
            if (expressions.Count != 1)
            {
                ctx.LogError($"qti-container-size should have exactly one child expression, found: {expressions.Count}");
                return null;
            }
            var container = expressions[0].Apply(ctx);
            var values = container.ToValueList();
            return (values == null ? 0 : values.Count).ToBaseValue();
        }
    }
}
