using Citolab.QTI.ScoringEngine.Helpers;
using Citolab.QTI.ScoringEngine.Interfaces;
using Citolab.QTI.ScoringEngine.Model;
using System;
using System.Collections.Generic;
using System.Text;
using System.Xml.Linq;

namespace Citolab.QTI.ScoringEngine.Expressions.GeneralExpressions
{
    internal class VariableResponseProcessing : IResponseProcessingExpression
    {
        public string Name => "qti-variable";

        public BaseValue Apply(XElement qtiElement, IProcessingContext ctx)
        {
            var identifier = qtiElement.Identifier();
            if (ctx.ResponseVariables != null && ctx.ResponseVariables.ContainsKey(identifier))
            {
                var responseVariable = ctx.ResponseVariables[identifier];
                var cardinality = Cardinality.Single;
                if (ctx.ResponseDeclarations.ContainsKey(identifier))
                {
                    cardinality = ctx.ResponseDeclarations[identifier].Cardinality;
                }
                return new BaseValue
                {
                    BaseType = responseVariable.BaseType,
                    Value = cardinality == Cardinality.Single ? responseVariable.Value : "",
                    Values = cardinality != Cardinality.Single ? responseVariable.Values : null,
                    Identifier = identifier,
                    Cardinality = cardinality
                };
            }
            else
            {
                if (ctx.OutcomeVariables != null && ctx.OutcomeVariables.ContainsKey(identifier))
                {
                    return ctx.OutcomeVariables[identifier].ToBaseValue();
                }
            }
            ctx.LogInformation($"Cannot find variable for identifier: {identifier}");
            return null;
        }
    }
}
