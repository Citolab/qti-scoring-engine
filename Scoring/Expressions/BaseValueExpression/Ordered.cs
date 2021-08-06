using Citolab.QTI.ScoringEngine.Expressions.ConditionExpressions;
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
            var baseValues = expressions.Select(e => e.Apply(ctx)).ToList();
            return new BaseValue
            {
                Identifier = "ordered",
                BaseType = baseValues.Any() ? baseValues[0].BaseType : BaseType.Identifier,
                Cardinality = Cardinality.Ordered,
                Values = baseValues?.Select(v => v.Value).ToList()
            };
        }

    }
}
