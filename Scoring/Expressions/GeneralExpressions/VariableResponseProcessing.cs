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
        private string _identifier;

        public string Name => "qti-variable";

        public BaseValue Apply(IProcessingContext ctx)
        {

            if (ctx.ResponseVariables != null && ctx.ResponseVariables.ContainsKey(_identifier))
            {
                var responseVariable = ctx.ResponseVariables[_identifier];
                var cardinality = Cardinality.Single;
                if (ctx.ResponseDeclarations.ContainsKey(_identifier))
                {
                    cardinality = ctx.ResponseDeclarations[_identifier].Cardinality;
                }
                return new BaseValue
                {
                    BaseType = responseVariable.BaseType,
                    Value = cardinality == Cardinality.Single ? responseVariable.Value : "",
                    Values = cardinality != Cardinality.Single ? responseVariable.Values : null,
                    Identifier = _identifier,
                    Cardinality = cardinality
                };
            }
            else
            {
                if (ctx.OutcomeVariables != null && ctx.OutcomeVariables.ContainsKey(_identifier))
                {
                    return ctx.OutcomeVariables[_identifier].ToBaseValue();
                }
            }
            ctx.LogInformation($"Cannot find variable for identifier: {_identifier}");
            return null;
        }

        public void Init(XElement qtiElement)
        {
            _identifier = qtiElement.Identifier();
        }
    }
}
