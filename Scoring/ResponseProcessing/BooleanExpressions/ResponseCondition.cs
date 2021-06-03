using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;
using Citolab.QTI.ScoringEngine.ResponseProcessing.Interfaces;

namespace Citolab.QTI.ScoringEngine.ResponseProcessing.BooleanExpressions
{
    internal class ResponseCondition : IBooleanExpression
    {
        public string Name { get => "qti-response-condition"; }

        public bool Execute(XElement qtiElement, ResponseProcessorContext context)
        {
            foreach (var child in qtiElement.Elements())
            {
                var executor = context.GetOperator(child, context);
                var result = executor?.Execute(child, context);
                if (result == true) // if handled then exit for;
                {
                    break;
                }
            }
            return true;
        }
    }
}
