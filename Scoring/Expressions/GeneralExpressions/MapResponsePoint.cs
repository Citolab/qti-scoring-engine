using Citolab.QTI.ScoringEngine.Helpers;
using Citolab.QTI.ScoringEngine.Interfaces;
using Citolab.QTI.ScoringEngine.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml.Linq;


namespace Citolab.QTI.ScoringEngine.Expressions.GeneralExpressions
{
    internal class MapResponsePoint : IResponseProcessingExpression
    {
        private string _identifier;

        public string Name => "qti-map-response-point";

        public BaseValue Apply(IProcessingContext ctx)
        {
            if (ctx.ResponseDeclarations.ContainsKey(_identifier) &&
               ctx.ResponseVariables.ContainsKey(_identifier))
            {
                var responseVariable = ctx.ResponseVariables[_identifier];

                var values = responseVariable.Values;
                if (responseVariable.Cardinality == Cardinality.Single)
                {
                    values = new List<string> { responseVariable.Value };
                }
                var responseDeclaration = ctx.ResponseDeclarations[_identifier];
                if (responseDeclaration.AreaMapping != null)
                {

                    var value = 0.0F;
                    foreach (var candidateValue in values)
                    {
                        var mappedValue = responseDeclaration.AreaMapping.AreaMappings.FirstOrDefault(m =>
                            Helper.IsInsideRegion(m.Coords, candidateValue, m.Shape, ctx));
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
                    return value.ToBaseValue();
                }
                else
                {
                    ctx.LogError($"mapResponse is mapped to responseDeclaration without mapping: {_identifier}");
                }
            }
            else
            {
                ctx.LogError($"Cannot find responseDeclaration with identifier: {_identifier} in mapReponse function.");
            }
            return 0.0F.ToBaseValue();
        }

        public void Init(XElement qtiElement)
        {
            _identifier = qtiElement.Identifier();
        }
    }
}
