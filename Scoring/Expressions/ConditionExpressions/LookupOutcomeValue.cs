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
    internal class LookupOutcomeValue : IConditionExpression
    {
        public string Name => "qti-lookup-outcome-value";

        public bool Execute(XElement qtiElement, IProcessingContext context)
        {
            var outcomeIdentifier = qtiElement.Identifier();
            var rawIdentifier = qtiElement.FindElementsByName("qti-variable")
                .FirstOrDefault()?.Identifier();
            if (rawIdentifier == null)
            {
                context.LogError("No outcome identifier could be found for the raw score to use with interpolation.");
                return false;
            }
            var rawOutcomeDeclaration = Helper.GetOutcomeDeclaration(rawIdentifier, context);
            var outcomeDeclaration = Helper.GetOutcomeDeclaration(outcomeIdentifier, context);
           
            if (outcomeDeclaration == null)
            {
                return false;
            }
            var rawOutcomeVariable = Helper.GetOutcomeVariable(rawIdentifier, rawOutcomeDeclaration, context, false);
            var outcomeVariable = Helper.GetOutcomeVariable(outcomeIdentifier, outcomeDeclaration, context);

            if (outcomeDeclaration.InterpolationTable == null)
            {
                context.LogError("Lookup refers to variable without interpolation table");
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
                    context.LogError($"Could not find lookup value: {value}");
                }
            }
            else
            {
                context.LogError($"Could not convert value: {value} to float. Cannot search for the interpolation value");
            }
            return false;

        }
    }
}
