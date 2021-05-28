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
            // TODO lower bound and upperbound if response contains multiple value when cardinality = multiple
            var identifier = qtiElement.Identifier();
            if (context.AssessmentItem.ResponseDeclarations.ContainsKey(identifier) &&
               context.ItemResult.ResponseVariables.ContainsKey(identifier))
            {
                var responseVariable = context.ItemResult.ResponseVariables[identifier];
                var responseDeclaration = context.AssessmentItem.ResponseDeclarations[identifier];
                if (responseDeclaration.Mapping != null)
                {
                    var mappedValue = responseDeclaration.Mapping.MapEntries.FirstOrDefault(m => m.MapKey == responseVariable.Value);
                    if (mappedValue == null)
                    {
                        return responseDeclaration.Mapping.DefaultValue;
                    }
                    else
                    {
                        var value = mappedValue.MappedValue;
   
                        return value;
                    }
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
