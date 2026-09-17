using Citolab.QTI.ScoringEngine.Expressions.ConditionExpressions;
using Citolab.QTI.ScoringEngine.Helpers;
using Citolab.QTI.ScoringEngine.Interfaces;
using Citolab.QTI.ScoringEngine.Model;
using System.Collections.Generic;
using System.Linq;

namespace Citolab.QTI.ScoringEngine.Expressions.BaseValueExpression
{
    /// <summary>
    /// qti-multiple: builds an unordered container from its children. Children that are
    /// containers themselves are flattened into it and NULL children are left out.
    /// </summary>
    internal class Multiple : ValueExpressionBase
    {
        public override BaseValue Apply(IProcessingContext ctx)
        {
            var baseValues = expressions.Select(e => e.Apply(ctx)).Where(v => v != null).ToList();
            var values = new List<string>();
            foreach (var baseValue in baseValues)
            {
                values.AddRange(baseValue.ToValueList());
            }
            return new BaseValue
            {
                Identifier = "multiple",
                BaseType = baseValues.Any() ? baseValues[0].BaseType : BaseType.Identifier,
                Cardinality = Cardinality.Multiple,
                Values = values
            };
        }
    }
}
