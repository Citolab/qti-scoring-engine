using Citolab.QTI.ScoringEngine.ResponseProcessing;
using Citolab.QTI.ScoringEngine.Helpers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;
using Citolab.QTI.ScoringEngine.Interfaces;

namespace Citolab.QTI.ScoringEngine.Expressions.ConditionExpressions
{
    internal class LookupOutcomeValue : ConditionExpressionBase
    {
        public override bool Execute(IProcessingContext ctx)
        {
            var outcomeIdentifier = GetAttributeValue("identifier");
            var rawOutcomeVariable = expressions.FirstOrDefault()?.Apply(ctx);
            if (rawOutcomeVariable == null)
            {
                ctx.LogError("No outcome identifier could be found for the raw score to use with interpolation.");
                return false;
            }
            var outcomeDeclaration = Helper.GetOutcomeDeclaration(outcomeIdentifier, ctx);

            if (outcomeDeclaration == null)
            {
                return false;
            }
             var outcomeVariable = Helper.GetOutcomeVariable(outcomeIdentifier, outcomeDeclaration, ctx);

            if (outcomeDeclaration.InterpolationTable == null)
            {
                ctx.LogError("Lookup refers to variable without interpolation table");
            }
            if (rawOutcomeVariable.Value.ToString().TryParseFloat(out var value))
            {
                var lookupEntry = outcomeDeclaration.InterpolationTable.FirstOrDefault(i => i.SourceValue == value);
                if (lookupEntry != null)
                {
                    outcomeVariable.Value = lookupEntry.TargetValue;
                    return true;
                }
                else
                {
                    ctx.LogError($"Could not find lookup value: {value}");
                }
            }
            else
            {
                ctx.LogError($"Could not convert value: {value} to float. Cannot search for the interpolation value");
            }
            return false;

        }
    }
}
