using Citolab.QTI.ScoringEngine.Helpers;
using Citolab.QTI.ScoringEngine.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;
using System.Globalization;
using Citolab.QTI.ScoringEngine.Interfaces;

namespace Citolab.QTI.ScoringEngine.Expressions.ConditionExpressions
{
    internal class SetOutcomeValue : ConditionExpressionBase
    {

        public override bool Execute(IProcessingContext ctx)
        {
            var outcomeIdentifier = GetAttributeValue("identifier");
            if (string.IsNullOrEmpty(outcomeIdentifier))
            {
                ctx.LogError($"Cannot find attribute identifier");
                return false;
            };
            if (!ctx.OutcomeDeclarations.ContainsKey(outcomeIdentifier))
            {
                ctx.LogError($"Cannot find outcomeDeclaration: {outcomeIdentifier}");
                return false;
            }
            else
            {
                var outcomeDeclaration = Helper.GetOutcomeDeclaration(outcomeIdentifier, ctx);
                var outcomeVariable = Helper.GetOutcomeVariable(outcomeIdentifier, outcomeDeclaration, ctx);

                if (expressions.Count() == 1)
                {
                    var childElement = expressions.FirstOrDefault();
                    var culture = CultureInfo.InvariantCulture;
                    var value = childElement.Apply(ctx);
                    if (value?.Fields != null)
                    {
                        // a record is held in its fields, there is no single value to write
                        outcomeVariable.Cardinality = Cardinality.Record;
                        outcomeVariable.Fields = value.Fields;
                        outcomeVariable.Value = null;
                    }
                    else if (outcomeDeclaration.IsContainer)
                    {
                        // a multiple or ordered outcome holds every member; a single value is a
                        // container of one and NULL (qti-null, an unset variable) empties it.
                        outcomeVariable.Cardinality = outcomeDeclaration.Cardinality;
                        outcomeVariable.Values = value.ToValueList()?.ToList() ?? new List<string>();
                        outcomeVariable.Value = null;
                    }
                    else
                    {
                        // a container written to a single outcome: only a container of one fits
                        var singleValue = value?.Value;
                        if (singleValue == null && value?.Values != null && value.Values.Count > 0)
                        {
                            if (value.Values.Count > 1)
                            {
                                ctx.LogError($"A container of {value.Values.Count} values cannot be written to single outcome: {outcomeIdentifier}. Using the first value.");
                            }
                            singleValue = value.Values[0];
                        }
                        outcomeVariable.Value = singleValue?.ToString(culture);
                    }
                }
                else
                {
                    ctx.LogError($"Unexpected childs: {expressions.Count()} in SetOutcomeValue");
                }
            }
            return true;
        }
    }
}
