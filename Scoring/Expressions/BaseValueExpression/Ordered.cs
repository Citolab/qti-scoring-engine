using Citolab.QTI.ScoringEngine.Expressions.ConditionExpressions;
using Citolab.QTI.ScoringEngine.Helpers;
using Citolab.QTI.ScoringEngine.Interfaces;
using Citolab.QTI.ScoringEngine.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml.Linq;

namespace Citolab.QTI.ScoringEngine.Expressions.BaseValueExpression
{
    internal class Ordered : ValueExpressionBase
    {
        public override BaseValue Apply(IProcessingContext ctx )
        {
            // like qti-multiple: NULL children are left out and containers are flattened, so
            // <qti-ordered><qti-variable identifier="O"/>...</qti-ordered> appends to O.
            var baseValues = expressions.Select(e => e.Apply(ctx)).Where(v => v != null).ToList();
            var values = new List<string>();
            foreach (var baseValue in baseValues)
            {
                values.AddRange(baseValue.ToValueList());
            }
            return new BaseValue
            {
                Identifier = "ordered",
                BaseType = baseValues.Any() ? baseValues[0].BaseType : BaseType.Identifier,
                Cardinality = Cardinality.Ordered,
                Values = values
            };
        }

    }
}
