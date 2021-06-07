using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;
using Citolab.QTI.ScoringEngine.Interfaces;

namespace Citolab.QTI.ScoringEngine.Expressions.ConditionExpressions
{
    internal class ResponseCondition : IResponseProcessingConditionExpression
    {
        public string Name { get => "qti-response-condition"; }

        public bool Execute(XElement qtiElement, IProcessingContext context)
        {
            foreach (var child in qtiElement.Elements())
            {
                var result = context.CheckCondition(child);
                if (result == true) // if handled then exit for;
                {
                    break;
                }
            }
            return true;
        }
    }
}
