using Microsoft.Extensions.Logging;
using Citolab.QTI.ScoringEngine.Helper;
using Citolab.QTI.ScoringEngine.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;

namespace Citolab.QTI.ScoringEngine.OutcomeProcessing
{
    internal static class OutcomeProcessor
    {
        public static AssessmentResult Process(AssessmentTest assessmentTest, AssessmentResult assessmentResult, ILogger logger)
        {
            var context = new OutcomeProcessorContext(assessmentResult, assessmentTest, logger);
            // Reset all values that are recalculated;
            context.TestResult.OutcomeVariables.Where(o =>
            {
                return assessmentTest.CalculatedOutcomes.Contains(o.Key);
            }).ToList()
            .ForEach(o =>
            {
                o.Value.Value = 0;
            });
            if (assessmentTest.OutcomeProcessingElement != null)
            {
                foreach (var outcomeProcessingChildElement in assessmentTest.OutcomeProcessingElement.Elements())
                {
                    var executor = context.GetOperator(outcomeProcessingChildElement, context);
                    executor?.Execute(outcomeProcessingChildElement, context);
                }
                assessmentTest.CalculatedOutcomes.ToList().ForEach(outcomeIdentifier =>
                {
                    assessmentResult.PersistTestResultOutcome(assessmentTest.Identifier, outcomeIdentifier);
                });
            }
            else
            {
                context.LogInformation("No qti-outcome-processing found");
            }
            return context.AssessmentResult;
        }



    }
}
