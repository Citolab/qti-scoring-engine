using Citolab.QTI.ScoringEngine.Expressions.ConditionExpressions;
using Citolab.QTI.ScoringEngine.Helpers;
using Citolab.QTI.ScoringEngine.Interfaces;
using Citolab.QTI.ScoringEngine.Model;
using System.Collections.Generic;
using System.Linq;

namespace Citolab.QTI.ScoringEngine.Expressions.BaseValueExpression
{
    /// <summary>
    /// qti-delete: the container of the second child without any occurrence of the single
    /// value of the first.
    /// </summary>
    internal class Delete : ValueExpressionBase
    {
        public override BaseValue Apply(IProcessingContext ctx)
        {
            if (expressions.Count != 2)
            {
                ctx.LogError($"qti-delete should have two child expressions, found: {expressions.Count}");
                return null;
            }
            var valueToDelete = expressions[0].Apply(ctx);
            var container = expressions[1].Apply(ctx);
            if (valueToDelete?.Value == null || container == null)
            {
                return null;
            }
            var values = container.ToValueList()
                .Where(value => !Helper.CompareSingleValues(value, valueToDelete.Value, container.BaseType, ctx))
                .ToList();
            return new BaseValue
            {
                Identifier = container.Identifier,
                BaseType = container.BaseType,
                Cardinality = container.Cardinality ?? Cardinality.Multiple,
                Values = values
            };
        }
    }
}
