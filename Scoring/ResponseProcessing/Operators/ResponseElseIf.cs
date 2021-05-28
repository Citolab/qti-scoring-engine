using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;
using Citolab.QTI.Scoring.ResponseProcessing.Interfaces;

namespace Citolab.QTI.Scoring.ResponseProcessing.Operators
{
    internal class ResponseElseIf : IResponseProcessingOperator
    {
        public string Name { get => "responseElseIf"; }

        public bool Execute(XElement qtiElement, ResponseProcessorContext context)
        {
            return new ResponseIf().Execute(qtiElement, context);
        }
    }
}
