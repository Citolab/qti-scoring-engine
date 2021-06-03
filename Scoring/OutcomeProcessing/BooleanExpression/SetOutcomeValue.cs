using Citolab.QTI.ScoringEngine.Helper;
using Citolab.QTI.ScoringEngine.ResponseProcessing;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;

namespace Citolab.QTI.ScoringEngine.OutcomeProcessing.BooleanExpression
{
    internal class SetOutcomeValue : IOutcomeBooleanExpression
    {
        public string Name => "qti-set-outcome-value";

        public bool Execute(XElement qtiElement, OutcomeProcessorContext context)
        {
            var outcomeIdentifier = qtiElement.Identifier();
            if (string.IsNullOrEmpty(outcomeIdentifier))
            {
                context.LogError($"Cannot find attribute identifier");
                return false;
            };
            var allTestVariables = context.TestResult.OutcomeVariables
                .ToDictionary(t => t.Key, t => t.Value);
            if (!allTestVariables.ContainsKey(outcomeIdentifier))
            {
                if (context.AssessmentTest.OutcomeDeclarations.ContainsKey(outcomeIdentifier))
                {
                    var outcomeDeclaration = context.AssessmentTest.OutcomeDeclarations[outcomeIdentifier];
                    var newVariable = outcomeDeclaration.ToVariable();
                    context.TestResult.OutcomeVariables.Add(newVariable.Identifier, newVariable);
                    allTestVariables.Add(newVariable.Identifier, newVariable);
                }
                else
                {
                    context.LogError($"cannot find outcomeDeclaration: {outcomeIdentifier}");
                    return false;
                }
            }
            var outcomeVariable = allTestVariables[outcomeIdentifier];
            // check if there is an operator. check value from baseValue
            var childElement = qtiElement.Elements().FirstOrDefault();
            var calculator = context.GetExpression(childElement, context);
            if (calculator != null)
            {
                outcomeVariable.Value = calculator.Calculate(childElement, context).Value;
            }
            else
            {
                var values = qtiElement.GetValues(context);
                if (values.Count > 1)
                {
                    context.LogError($"Unexpected number of values in SetOutcomeValue: {values.Count }");
                }
                else
                {
                    outcomeVariable.Value = values.Count == 0 ? "" : values.FirstOrDefault().Value;
                }
            }
            //}
            return true;
        }
    }
}
