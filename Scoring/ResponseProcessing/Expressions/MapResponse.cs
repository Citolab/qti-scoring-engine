using Citolab.QTI.Scoring.Helper;
using Citolab.QTI.Scoring.Model;
using Citolab.QTI.Scoring.ResponseProcessing.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;

namespace Citolab.QTI.Scoring.ResponseProcessing.Expressions
{
    internal class MapResponse : IResponseProcessingExpression
    {
        public string Name => "mapResponse";

        public float Calculate(XElement qtiElement, ResponseProcessorContext context)
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
                    foreach(var candidateValue in values)
                    {
                        var mappedValue = responseDeclaration.Mapping.MapEntries.FirstOrDefault(m => m.MapKey == candidateValue);
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
                    return value;
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
            return 0.0F;
        }

    }
}
