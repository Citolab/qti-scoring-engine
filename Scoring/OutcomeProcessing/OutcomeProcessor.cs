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
        public static AssessmentResult Process(AssessmentTest assessmentTest, AssessmentResult assessmentResult, ILogger logger)
        {
            var ctx = new OutcomeProcessorContext(assessmentResult, assessmentTest, logger);
            // Reset all values that are recalculated;
            ctx.TestResult.OutcomeVariables.Where(o =>
            {
                return assessmentTest.CalculatedOutcomes.Contains(o.Key);
            }).ToList()
            .ForEach(o =>
            {
                o.Value.Value = 0;
            });
            if (assessmentTest.Expressions  != null && assessmentTest.Expressions.Count > 0)
            {
                foreach (var conditionalExpressions in assessmentTest.Expressions)
                {
                    conditionalExpressions.Execute(ctx);
                }
                assessmentTest.CalculatedOutcomes.ToList().ForEach(outcomeIdentifier =>
                {
                    assessmentResult.PersistTestResultOutcome(assessmentTest.Identifier, outcomeIdentifier);
                });
            }
            else
            {
                ctx.LogInformation("No qti-outcome-processing found");
            }
            return ctx.AssessmentResult;
        }
    }
}
