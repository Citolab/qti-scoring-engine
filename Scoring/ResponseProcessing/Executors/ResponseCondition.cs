using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;
using Citolab.QTI.ScoringEngine.ResponseProcessing.Interfaces;

namespace Citolab.QTI.ScoringEngine.ResponseProcessing.Executors
{
    public class ResponseCondition : IExecuteReponseProcessing
    {
        public string Name { get => "responseCondition"; }

        bool IExecuteReponseProcessing.Execute(XElement qtiElement, ResponseProcessorContext context)
        {
            foreach (var child in qtiElement.Elements())
            {
                var executor = ExecuteFactory.GetExecutor(child, context);
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
