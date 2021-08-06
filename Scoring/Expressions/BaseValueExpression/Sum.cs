using Citolab.QTI.ScoringEngine.ResponseProcessing;
using Citolab.QTI.ScoringEngine.Helpers;
using Citolab.QTI.ScoringEngine.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;
using Citolab.QTI.ScoringEngine.Interfaces;
using Citolab.QTI.ScoringEngine.Expressions.ConditionExpressions;

namespace Citolab.QTI.ScoringEngine.OutcomeProcessing.BaseValueExpression
{
    internal class Sum : ValueExpressionBase
    {
        public override BaseValue Apply(IProcessingContext ctx)
    {
        var baseValues = expressions.Select(e => e.Apply(ctx)).ToList();
        var values = baseValues.Select(baseValue =>
            {
                float result = 0.0f;
                var canParse = baseValue.Value.TryParseFloat(out result);
                if (canParse == true)
                {
                    return result;
                }
                else
                {
                    return 0.0f;
                }
            }).ToList();
            var sum = values.Sum();
            return ((float)sum).ToBaseValue();
        }

    }
}
