using Citolab.QTI.ScoringEngine.Expressions.ConditionExpressions;
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
    internal class Correct : ValueExpressionBase
    {
        public override BaseValue Apply(IProcessingContext ctx)
        {
            var identifier = GetAttributeValue("identifier");
            if (ctx.ResponseDeclarations.ContainsKey(identifier))
            {
                var dec = ctx.ResponseDeclarations[identifier];
                if (dec.Cardinality == Cardinality.Single && string.IsNullOrWhiteSpace(dec.CorrectResponse))
                {
                    ctx.LogError($"Correct: {identifier} references to a response without correctResponse");
                    return null;
                }
                if (dec.Cardinality != Cardinality.Single && (dec.CorrectResponses == null || !dec.CorrectResponses.Any()))
                {
                    ctx.LogError($"Correct: {identifier} references to a response without correctResponse");
                    return null;
                }
                return new BaseValue { Identifier = identifier, BaseType = dec.BaseType, Value = dec.CorrectResponse, Values = dec.CorrectResponses, Cardinality = dec.Cardinality };
            }
            else
            {
                ctx.LogError($"Cannot reference to response declaration for correct {identifier}");
                return null;
            }
        }
   
    }
}
