using Citolab.QTI.ScoringEngine.Helper;
using Citolab.QTI.ScoringEngine.Model;
using Citolab.QTI.ScoringEngine.ResponseProcessing.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;

namespace Citolab.QTI.ScoringEngine.ResponseProcessing.BaseValueExpressions
{
    internal class MapResponse : IBaseValueExpression
    {
        public string Name => "mapResponse";

        public BaseValue Calculate(XElement qtiElement, ResponseProcessorContext context)
        {
            var identifier = qtiElement.Identifier();
            if (context.AssessmentItem.ResponseDeclarations.ContainsKey(identifier) &&
               context.ItemResult.ResponseVariables.ContainsKey(identifier))
            {
                var responseVariable = context.ItemResult.ResponseVariables[identifier];

                var values = responseVariable.Values;
                if (responseVariable.Cardinality == Cardinality.Single)
                {
                    values = new List<string> { responseVariable.Value };
                }
                var responseDeclaration = context.AssessmentItem.ResponseDeclarations[identifier];
                if (responseDeclaration.Mapping != null)
                {

                    var value = 0.0F;
                    foreach (var candidateValue in values)
                    {
                        var mappedValue = responseDeclaration.Mapping.MapEntries.FirstOrDefault(m =>
                        Helper.CompareSingleValues(m.MapKey, candidateValue, responseDeclaration.BaseType, context));
                        if (mappedValue == null)
                        {
                            value += responseDeclaration.Mapping.DefaultValue;
                        }
                        else
                        {
                            value += mappedValue.MappedValue;
                        }
                    }
                    if (responseDeclaration.Mapping.LowerBound.HasValue)
                    {
                        value = Math.Max(responseDeclaration.Mapping.LowerBound.Value, value);
                    }
                    if (responseDeclaration.Mapping.UpperBound.HasValue)
                    {
                        value = Math.Min(responseDeclaration.Mapping.UpperBound.Value, value);
                    }
                    return value.ToBaseValue();
                }
                else
                {
                    context.LogError($"mapResponse is mapped to responseDeclaration without mapping: {identifier}");
                }
            }
            else
            {
                context.LogError($"Cannot find responseDeclaration with identifier: {identifier} in mapReponse function.");
            }
            return 0.0F.ToBaseValue();
        }

    }
}
