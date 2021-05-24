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

namespace Citolab.QTI.ScoringEngine.ResponseProcessing.Calculators
{
    public class Sum : ICalculateResponseProcessing
    {
        public string Name => "sum";

        public float Calculate(XElement qtiElement, ResponseProcessingContext context)
        {
            var sum = qtiElement.GetValues(context).Select(value =>
            {
                if (float.TryParse(value.Value, out var result))
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
