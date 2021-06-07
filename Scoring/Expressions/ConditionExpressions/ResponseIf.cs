using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;
using Citolab.QTI.ScoringEngine.Interfaces;

namespace Citolab.QTI.ScoringEngine.Expressions.ConditionExpressions
{
    internal class ResponseIf : IResponseProcessingConditionExpression
    {
        public string Name { get => "qti-response-if"; }

        public bool Execute(XElement qtiElement, IProcessingContext context)
        {
            var firstChild = qtiElement.Elements().FirstOrDefault();
            var result = context.CheckCondition(firstChild);
            if (result == true)
            {
                var otherChilds = qtiElement.Elements().Skip(1);
                var maxLoops = otherChilds.Count() >= 100 ? 100 : otherChilds.Count();
                foreach (var otherChild in otherChilds.Take(maxLoops))
                {
                    context.Execute(otherChild);
                }
                return true;
            }
            return false;
        }
    }
}
