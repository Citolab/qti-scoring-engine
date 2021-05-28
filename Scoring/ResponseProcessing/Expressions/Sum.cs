using Citolab.QTI.Scoring.ResponseProcessing;
using Citolab.QTI.Scoring.Helper;
using Citolab.QTI.Scoring.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;
using Citolab.QTI.Scoring.ResponseProcessing.Interfaces;

namespace Citolab.QTI.Scoring.ResponseProcessing.Expressions
{
    internal class Sum : IResponseProcessingExpression
    {
        public string Name => "sum";

        public float Calculate(XElement qtiElement, ResponseProcessorContext context)
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
            return (float)sum;
        }
    }
}
