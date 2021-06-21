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
    internal class Correct : IResponseProcessingExpression
    {
        private string _identifier;

        public string Name => "qti-correct";
        public BaseValue Apply(IProcessingContext ctx)
        {
      
            if (ctx.ResponseDeclarations.ContainsKey(_identifier))
            {

                var dec = ctx.ResponseDeclarations[_identifier];
                if (dec.Cardinality == Cardinality.Single && string.IsNullOrWhiteSpace(dec.CorrectResponse))
                {
                    ctx.LogError($"Correct: {_identifier} references to a response without correctResponse");
                    return null;
                }
                if (dec.Cardinality != Cardinality.Single && (dec.CorrectResponses == null || !dec.CorrectResponses.Any()))
                {
                    ctx.LogError($"Correct: {_identifier} references to a response without correctResponse");
                    return null;
                }
                return new BaseValue { Identifier = _identifier, BaseType = dec.BaseType, Value = dec.CorrectResponse, Values = dec.CorrectResponses, Cardinality = dec.Cardinality };
            }
            else
            {
                ctx.LogError($"Cannot reference to response declaration for correct {_identifier}");
                return null;
            }
        }


        public void Init(XElement qtiElement)
        {
            _identifier = qtiElement.Identifier();
        }
    }
}
