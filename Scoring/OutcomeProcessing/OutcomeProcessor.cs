using Microsoft.Extensions.Logging;
using Citolab.QTI.ScoringEngine.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using Citolab.QTI.ScoringEngine.Interfaces;

namespace Citolab.QTI.ScoringEngine.OutcomeProcessing
{
    internal static class OutcomeProcessor
    {
        public static AssessmentResult Process(AssessmentTest assessmentTest, AssessmentResult assessmentResult, ILogger logger, List<ICustomOperator> customOperators = null)
        {
            var context = new OutcomeProcessorContext(assessmentResult, assessmentTest, logger, customOperators);
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
                    context.Execute(outcomeProcessingChildElement);
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
