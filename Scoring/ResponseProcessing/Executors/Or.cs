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
    /// One of the children should return true
    /// </summary>
    internal class Or : IExecuteReponseProcessing
    {
        public string Name => "or";

        public bool Execute(XElement qtiElement, ResponseProcessorContext context)
        {
            foreach (var child in qtiElement.Elements())
            {
                var executor = context.GetExecutor(child, context);
                var result = executor?.Execute(child, context);
                if (result == true)
                {
                    return true; // one is true, return true
                }
            }
            return false; // all false: return false
        }

    }
}
