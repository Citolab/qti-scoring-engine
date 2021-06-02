using Citolab.QTI.ScoringEngine.ResponseProcessing;
using Citolab.QTI.ScoringEngine.Helper;
using Citolab.QTI.ScoringEngine.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;
using Citolab.QTI.ScoringEngine.ResponseProcessing.Interfaces;

namespace Citolab.QTI.ScoringEngine.ResponseProcessing.BaseValueExpressions
{
    internal class Sum : IBaseValueExpression
    {
        public string Name => "sum";

        public BaseValue Calculate(XElement qtiElement, ResponseProcessorContext context)
        {
            var sum = qtiElement.GetValues(context).Select(value =>
            {
                if (value.Value.TryParseFloat(out var result))
                {
                    return result;
                }
                else
                {
                    context.LogError($"Cannot cast outcomeValue: {value.Value} of baseType: {value.BaseType} to a float to sum.");
                    return 0.0;
                }
            }).Sum();
            return ((float)sum).ToBaseValue();
        }
    }
}
