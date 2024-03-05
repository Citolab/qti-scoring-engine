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
    internal class Min : ValueExpressionBase
    {
        public override BaseValue Apply(IProcessingContext ctx)
        {
            var baseValues = expressions?.Select(e => e.Apply(ctx))?.ToList();
            if (baseValues == null)
            {
                return 0.0f.ToBaseValue();
            }
            // get max value of the list
            return baseValues.Select(b =>
            {
                if (b.Value.TryParseFloat(out var result))
                {
                    return result;
                }
                return 0.0f;
            }).Min().ToBaseValue();
        }

    }
}
