using System;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;
using Citolab.QTI.Scoring.Helper;
using Citolab.QTI.Scoring.ResponseProcessing.Interfaces;

namespace Citolab.QTI.Scoring.ResponseProcessing.Executors
{
    internal class Equal : IExecuteReponseProcessing
    {
        public string Name { get => "equal"; }

        public bool Execute(XElement qtiElement, ResponseProcessorContext context)
        {
            // todo toleranceMode mode, couldn't find any example of absolute or relative.
            var toleranceMode = qtiElement.GetAttributeValue("toleranceMode");
            if (!string.IsNullOrEmpty(toleranceMode) && toleranceMode != "excat")
            {
                context.LogError($"Unsupported toleranceMode: {toleranceMode}");
            }
            return Helper.CompareTwoValues(qtiElement, context, Model.BaseType.Float);
        }

    }
}