using System;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;
using Citolab.QTI.ScoringEngine.Helpers;
using Citolab.QTI.ScoringEngine.Interfaces;

namespace Citolab.QTI.ScoringEngine.Expressions.ConditionExpressions
{
    internal class Equal : IConditionExpression
    {
        public string Name { get => "qti-equal"; }

        public bool Execute(XElement qtiElement, IProcessingContext context)
        {
            // todo toleranceMode mode, couldn't find any example of absolute or relative.
            var toleranceMode = qtiElement.GetAttributeValue("tolerance-mode");
            if (!string.IsNullOrEmpty(toleranceMode) && toleranceMode != "exact")
            {
                context.LogError($"Unsupported tolerance-mode: {toleranceMode}");
                return false;
            }
            return Helpers.Helper.CompareTwoValues(qtiElement, context, Model.BaseType.Float);
        }

    }
}