using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;
using Citolab.QTI.ScoringEngine.ResponseProcessing.Interfaces;

namespace Citolab.QTI.ScoringEngine.ResponseProcessing.BooleanExpressions
{
    internal class ResponseElseIf : IBooleanExpression
    {
        public string Name { get => "qti-response-else-if"; }

        public bool Execute(XElement qtiElement, ResponseProcessorContext context)
        {
            return new ResponseIf().Execute(qtiElement, context);
        }
    }
}
