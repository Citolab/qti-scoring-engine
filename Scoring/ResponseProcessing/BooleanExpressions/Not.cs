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

namespace Citolab.QTI.ScoringEngine.ResponseProcessing.BooleanExpressions
{
    /// <summary>
    /// All children should return true
    /// </summary>
    internal class Not : IBooleanExpression
    {
        public string Name => "not";

        public bool Execute(XElement qtiElement, ResponseProcessorContext context)
        {
            var elements = qtiElement.Elements();
            if (elements.Count() != 1)
            {
                context.LogError("Not element should contain only one child");
                return true;
            }
            var executor = context.GetOperator(elements.FirstOrDefault(), context);
            var result = executor?.Execute(elements.FirstOrDefault(), context);
            return !(result.HasValue && result.Value);
        }
    }
}
