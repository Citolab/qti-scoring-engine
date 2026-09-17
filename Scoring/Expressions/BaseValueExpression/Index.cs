using Citolab.QTI.ScoringEngine.Expressions.ConditionExpressions;
using Citolab.QTI.ScoringEngine.Interfaces;
using Citolab.QTI.ScoringEngine.Model;
using System.Linq;

namespace Citolab.QTI.ScoringEngine.Expressions.BaseValueExpression
{
    /// <summary>
    /// qti-index: takes the value at position n (1-based) from an ordered container.
    /// Returns null when n is outside the container, as the spec prescribes.
    /// </summary>
    internal class Index : ValueExpressionBase
    {
        public override BaseValue Apply(IProcessingContext ctx)
        {
            var n = GetAttributeValue("n");
            if (!int.TryParse(n, out var position) || position < 1)
            {
                ctx.LogError($"qti-index has a missing or invalid n attribute: '{n}'");
                return null;
            }
            if (expressions.Count != 1)
            {
                ctx.LogError("qti-index should have exactly one child expression.");
                return null;
            }
            var baseValue = expressions[0].Apply(ctx);
            if (baseValue == null)
            {
                return null;
            }
            // a container exposes its members in Values; a single value has none to index into.
            var values = baseValue.Values;
            if (values == null || position > values.Count)
            {
                ctx.LogInformation($"qti-index n={position} is outside the container, returning null.");
                return null;
            }
            return new BaseValue
            {
                Identifier = baseValue.Identifier,
                BaseType = baseValue.BaseType,
                Cardinality = Cardinality.Single,
                Value = values[position - 1]
            };
        }
    }
}
