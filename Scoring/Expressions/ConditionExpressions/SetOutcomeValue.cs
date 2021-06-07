using Citolab.QTI.ScoringEngine.Helpers;
using Citolab.QTI.ScoringEngine.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;
using System.Globalization;
using Citolab.QTI.ScoringEngine.Interfaces;

namespace Citolab.QTI.ScoringEngine.Expressions.ConditionExpressions
{
    internal class SetOutcomeValue : IConditionExpression
    {
        public string Name => "qti-set-outcome-value";

        public bool Execute(XElement qtiElement, IProcessingContext context)
        {
            var outcomeIdentifier = qtiElement.Identifier();
            if (string.IsNullOrEmpty(outcomeIdentifier))
            {
                context.LogError($"Cannot find attribute identifier");
                return false;
            };
            if (!context.OutcomeDeclarations.ContainsKey(outcomeIdentifier))
            {
                context.LogError($"Cannot find outcomeDeclaration: {outcomeIdentifier}");
                return false;
            }
            else
            {
                var outcomeDeclaration = Helper.GetOutcomeDeclaration(outcomeIdentifier, context);
                var outcomeVariable = Helper.GetOutcomeVariable(outcomeIdentifier, outcomeDeclaration, context);

                if (qtiElement.Elements().Count() == 1)
                {
                    var childElement = qtiElement.Elements().FirstOrDefault();
                    var culture = CultureInfo.InvariantCulture;
                    outcomeVariable.Value = context.GetValue(childElement)?.Value.ToString(culture);
                }
                else
                {
                    context.LogError($"Unexpected childs: {qtiElement.Elements().Count()} in SetOutcomeValue");
                }
            }
            return true;
        }
    }
}
