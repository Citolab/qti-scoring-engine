using Citolab.QTI.ScoringEngine.Helper;
using Citolab.QTI.ScoringEngine.Model;
using Citolab.QTI.ScoringEngine.ResponseProcessing.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;

namespace Citolab.QTI.ScoringEngine.ResponseProcessing.Expressions
{
    internal class MapResponsePoint : IResponseProcessingExpression
    {
        public string Name => "mapResponsePoint";

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
                if (responseDeclaration.AreaMapping != null)
                {

                    var value = 0.0F;
                    foreach (var candidateValue in values)
                    {
                        var mappedValue = responseDeclaration.AreaMapping.AreaMappings.FirstOrDefault(m =>
                            Helper.IsInsideRegion(m.Coords, candidateValue, m.Shape, context));
                        if (mappedValue == null)
                        {
                            value += responseDeclaration.AreaMapping.DefaultValue;
                        }
                        else
                        {
                            value += mappedValue.MappedValue;
                        }
                    }
                    if (responseDeclaration.AreaMapping.LowerBound.HasValue)
                    {
                        value = Math.Max(responseDeclaration.AreaMapping.LowerBound.Value, value);
                    }
                    if (responseDeclaration.AreaMapping.UpperBound.HasValue)
                    {
                        value = Math.Min(responseDeclaration.AreaMapping.UpperBound.Value, value);
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
