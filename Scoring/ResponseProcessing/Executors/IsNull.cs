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

namespace Citolab.QTI.ScoringEngine.ResponseProcessing.Executors
{
    /// <summary>
    /// null check
    /// </summary>
    public class IsNull : IExecuteReponseProcessing
    {
        public string Name => "isNull";

        public bool Execute(XElement qtiElement, ResponseProcessorContext context)
        {
            var value = qtiElement.GetValues(context).FirstOrDefault();
            return value == null;
        }
    }
}
