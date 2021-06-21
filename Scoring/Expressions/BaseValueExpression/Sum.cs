using Citolab.QTI.ScoringEngine.ResponseProcessing;
using Citolab.QTI.ScoringEngine.Helpers;
using Citolab.QTI.ScoringEngine.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;
using Citolab.QTI.ScoringEngine.Interfaces;

namespace Citolab.QTI.ScoringEngine.OutcomeProcessing.BaseValueExpression
{
    internal class Sum : IExpression
    {
        public string Name => "qti-sum";

        public BaseValue Apply(IProcessingContext ctx)
        {
            var values = childExpressions.Select(expression => expression.Apply())
           //     qtiElement.Elements().Select(element =>
           //{
           //    var baseValue = ctx.GetValue(element);
           //    if (float.TryParse(baseValue?.Value, out var result))
           //    {
           //        return result;
           //    }
           //    else
           //    {
           //        ctx.LogError($"Cannot cast outcome-value: {baseValue.Value} of base-type: {baseValue.BaseType} to a float to sum.");
           //        return 0.0f;
           //    }
           //});
            var sum = values.Sum();
            return ((float)sum).ToBaseValue();
        }

        public void Init(XElement qtiElement)
        {
            var childExpressions = qtiElement.Elements();
            throw new NotImplementedException();
        }
    }
}
