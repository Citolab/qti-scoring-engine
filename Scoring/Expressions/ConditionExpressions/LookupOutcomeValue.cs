using Citolab.QTI.ScoringEngine.ResponseProcessing;
using Citolab.QTI.ScoringEngine.Helpers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;
using System.Globalization;
using Citolab.QTI.ScoringEngine.Model;
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

            if (outcomeDeclaration.MatchTable != null)
            {
                // a match table maps an exact source value, so it is looked up as written and
                // only compared as a number when both sides are one.
                var rawValue = rawOutcomeVariable.Value?.ToString();
                var matchEntry = outcomeDeclaration.MatchTable.FirstOrDefault(m => IsSameSourceValue(m.SourceValue, rawValue));
                if (matchEntry != null)
                {
                    SetLookedUpValue(outcomeVariable, matchEntry.TargetValue);
                    return true;
                }
                ctx.LogError($"Could not find lookup value: {rawValue} in the match table of {outcomeIdentifier}");
                return false;
            }
            if (outcomeDeclaration.InterpolationTable == null)
            {
                ctx.LogError($"Lookup refers to variable without interpolation or match table: {outcomeIdentifier}");
                return false;
            }
            // a container (or NULL) has no single value to look up
            var rawNumber = rawOutcomeVariable.Value?.ToString();
            if (rawNumber != null && rawNumber.TryParseFloat(out var value))
            {
                var lookupEntry = outcomeDeclaration.InterpolationTable.FirstOrDefault(i => i.SourceValue == value);
                if (lookupEntry != null)
                {
                    SetLookedUpValue(outcomeVariable, lookupEntry.TargetValue);
                    return true;
                }
                else
                {
                    ctx.LogError($"Could not find lookup value: {value}");
                }
            }
            else
            {
                ctx.LogError($"Could not convert value: {rawOutcomeVariable.Value} to float. Cannot search for the interpolation value");
            }
            return false;
        }

        /// <summary>
        /// A multiple or ordered outcome keeps its members in Values: the value looked up
        /// becomes a container of one.
        /// </summary>
        private static void SetLookedUpValue(OutcomeVariable outcomeVariable, object targetValue)
        {
            if (outcomeVariable.IsContainer)
            {
                var member = Convert.ToString(targetValue, CultureInfo.InvariantCulture);
                outcomeVariable.Values = member == null ? new List<string>() : new List<string> { member };
                outcomeVariable.Value = null;
                return;
            }
            outcomeVariable.Value = targetValue;
        }

        private static bool IsSameSourceValue(string sourceValue, string rawValue)
        {
            if (sourceValue == null || rawValue == null)
            {
                return false;
            }
            if (sourceValue == rawValue)
            {
                return true;
            }
            return sourceValue.TryParseFloat(out var source) &&
                   rawValue.TryParseFloat(out var raw) &&
                   source == raw;

        }
    }
}
