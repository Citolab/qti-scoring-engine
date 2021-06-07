using Citolab.QTI.ScoringEngine.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;

namespace Citolab.QTI.ScoringEngine.Expressions.ConditionExpressions
{
    internal class ResponseElse : IResponseProcessingConditionExpression
    {
        public string Name { get => "qti-response-else"; }

        public bool Execute(XElement qtiElement, IProcessingContext context)
        {
            var elements = qtiElement.Elements();
            var maxLoops = elements.Count() >= 100 ? 100 : elements.Count();
            foreach (var child in qtiElement.Elements().Take(maxLoops)) // get a max value to
            {
                context.Execute(child);
            }
            return true;
        }
    }
}
