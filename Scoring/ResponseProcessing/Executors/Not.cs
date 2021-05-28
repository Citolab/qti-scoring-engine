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
    internal class Not : IExecuteReponseProcessing
    {
        public string Name => "not";

        public bool Execute(XElement qtiElement, ResponseProcessorContext context)
        {
            var elements = qtiElement.Elements();
            if (elements.Count() != 1)
            {
                context.LogError("Not element should contain only one child");
                return true;
            }
            var executor = context.GetExecutor(elements.FirstOrDefault(), context);
            var result = executor?.Execute(elements.FirstOrDefault(), context);
            return !(result.HasValue && result.Value);
        }
    }
}
