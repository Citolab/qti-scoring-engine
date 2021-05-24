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
    /// One of the children should return true
    /// </summary>
    public class Or : IExecuteReponseProcessing
    {
        public string Name => "or";

        public bool Execute(XElement qtiElement, ResponseProcessingContext context)
        {
            foreach (var child in qtiElement.Elements())
            {
                var executor = ExecuteFactory.GetExecutor(child, context);
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
