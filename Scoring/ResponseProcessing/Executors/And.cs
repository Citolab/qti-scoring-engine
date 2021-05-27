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

namespace Citolab.QTI.Scoring.ResponseProcessing.Executors
{
    /// <summary>
    /// All children should return true
    /// </summary>
    internal class And : IExecuteReponseProcessing
    {
        public string Name => "and";

        public bool Execute(XElement qtiElement, ResponseProcessorContext context)
        {
            foreach (var child in qtiElement.Elements())
            {
                var executor = context.GetExecutor(child, context);
                var result = executor?.Execute(child, context);
                if (result == false) 
                {
                    return false; // one condition false; return false
                }
            }
            return true; // all children true; return true
        }

    }
}
