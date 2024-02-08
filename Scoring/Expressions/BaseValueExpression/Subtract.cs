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
using System.Security.Cryptography;

namespace Citolab.QTI.ScoringEngine.OutcomeProcessing.BaseValueExpression
{
    internal class Subtract : ValueExpressionBase
    {
        public override BaseValue Apply(IProcessingContext ctx)
        {
            var baseValues = expressions.Select(e => e.Apply(ctx)).ToList();
            if (baseValues.Count == 2)
            {
                var value1 = baseValues[0]?.Value;
                var value2 = baseValues[1]?.Value;
                if (value1 == null || value2 == null)
                {
                    return 0.0f.ToBaseValue();
                }
                var result = (float)Convert.ToDouble(value1) - (float)Convert.ToDouble(value2);
                return result.ToBaseValue();
            }
            else
            {
                ctx.LogError("qti-subtract need to have 2 values");
                return 0.0f.ToBaseValue();
            }
        }

    }
}
