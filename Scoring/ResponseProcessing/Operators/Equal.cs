using System;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;
using Citolab.QTI.Scoring.Helper;
using Citolab.QTI.Scoring.ResponseProcessing.Interfaces;

namespace Citolab.QTI.Scoring.ResponseProcessing.Operators
{
    internal class Equal : IResponseProcessingOperator
    {
        public string Name { get => "equal"; }

        public bool Execute(XElement qtiElement, ResponseProcessorContext context)
        {
            // todo toleranceMode mode, couldn't find any example of absolute or relative.
            var toleranceMode = qtiElement.GetAttributeValue("toleranceMode");
            if (!string.IsNullOrEmpty(toleranceMode) && toleranceMode != "exact")
            {
                context.LogError($"Unsupported toleranceMode: {toleranceMode}");
                return false;
            }
            return Helper.CompareTwoValues(qtiElement, context, Model.BaseType.Float);
        }

    }
}