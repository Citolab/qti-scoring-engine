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

namespace Citolab.QTI.Scoring.ResponseProcessing.Operators
{
    /// <summary>
    /// null check
    /// </summary>
    internal class IsNull : IResponseProcessingOperator
    {
        public string Name => "isNull";

        public bool Execute(XElement qtiElement, ResponseProcessorContext context)
        {
            var value = qtiElement.GetValues(context).FirstOrDefault();
            return value == null;
        }
    }
}
