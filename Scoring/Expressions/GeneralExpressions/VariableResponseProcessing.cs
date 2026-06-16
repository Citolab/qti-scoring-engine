using Citolab.QTI.ScoringEngine.Expressions.ConditionExpressions;
using Citolab.QTI.ScoringEngine.Helpers;
using Citolab.QTI.ScoringEngine.Interfaces;
using Citolab.QTI.ScoringEngine.Model;
using System;
using System.Collections.Generic;
using System.Text;
using System.Xml.Linq;

namespace Citolab.QTI.ScoringEngine.Expressions.GeneralExpressions
{
    internal class VariableResponseProcessing : ValueExpressionBase
    {

        public override List<ProcessingType> UnsupportedProcessingTypes => new List<ProcessingType> { ProcessingType.OutcomeProcessing };


        public override BaseValue Apply(IProcessingContext ctx)
        {
            var identifer = GetAttributeValue("identifier");
            if (ctx.ResponseVariables != null && ctx.ResponseVariables.ContainsKey(identifer))
            {
                var responseVariable = ctx.ResponseVariables[identifer];
                var cardinality = Cardinality.Single;
                if (ctx.ResponseDeclarations.ContainsKey(identifer))
                {
                    cardinality = ctx.ResponseDeclarations[identifer].Cardinality;
                }
                return new BaseValue
                {
                    BaseType = responseVariable.BaseType,
                    Value = cardinality == Cardinality.Single ? responseVariable.Value : "",
                    // For multiple cardinality always expose the full list of values, even when it
                    // contains a single value (e.g. a gapMatch with one correct pair). The Count > 1
                    // check is a WORKAROUND FOR INVALID RESPONSE DECLARATION that only applies to
                    // single-declared responses that unexpectedly carry multiple values.
                    Values = cardinality == Cardinality.Single
                        ? (responseVariable.Values?.Count > 1 ? responseVariable.Values : null)
                        : responseVariable.Values,
                    Identifier = identifer,
                    Cardinality = cardinality
                };
            }
            else
            {
                if (ctx.OutcomeVariables != null && ctx.OutcomeVariables.ContainsKey(identifer))
                {
                    return ctx.OutcomeVariables[identifer].ToBaseValue();
                }
            }
            ctx.LogInformation($"Cannot find variable for identifier: {identifer}");
            return null;
        }

    }
}
