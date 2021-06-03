using Citolab.QTI.ScoringEngine.ResponseProcessing;
using Citolab.QTI.ScoringEngine.Helper;
using Citolab.QTI.ScoringEngine.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;

namespace Citolab.QTI.ScoringEngine.OutcomeProcessing.BaseValueExpression
{
    internal class Sum : IOutcomeBaseValueExpression
    {
        public string Name => "qti-sum";

        public BaseValue Calculate(XElement qtiElement, OutcomeProcessorContext context)
        {
            var sum = qtiElement.GetValues(context).Select(value =>
           {
               if (float.TryParse(value.Value, out var result))
               {
                   return result;
               }
               else
               {
                   context.LogError($"Cannot cast outcome-value: {value.Value} of base-type: {value.BaseType} to a float to sum.");
                   return 0.0f;
               }
           }).Sum();
            return ((float)sum).ToBaseValue();
        }
    }
}
