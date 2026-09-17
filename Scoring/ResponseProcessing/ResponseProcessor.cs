using Microsoft.Extensions.Logging;
using Citolab.QTI.ScoringEngine.ResponseProcessing;
using Citolab.QTI.ScoringEngine.Helpers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;
using Citolab.QTI.ScoringEngine.Model;
using Citolab.QTI.ScoringEngine.Interfaces;

namespace Citolab.QTI.ScoringEngine.ResponseProcessing
{
    internal static class ResponseProcessor
    {
        internal static AssessmentResult Process(AssessmentItem assessmentItem, AssessmentResult assessmentResult, ILogger logger, ResponseProcessingScoringsOptions options = null)
        {
            var ctx = new ResponseProcessorContext(logger, assessmentResult, assessmentItem, options);
            // Skip processing when there is no itemResult
            var containsItemResult = ctx.ItemResult != null;

            // Reset all values that are recalculated;
            // Give the default value
            if (containsItemResult)
            {
                // loop through condition executors
                if (assessmentItem.Expressions != null && assessmentItem.Expressions.Count > 0)
                {
                    try
                    {
                        foreach (var conditionalExpressions in assessmentItem.Expressions)
                        {
                            conditionalExpressions.Execute(ctx);
                        }
                    }
                    catch (ExitResponseException)
                    {
                        // qti-exit-response: the rules that are left are skipped, but whatever
                        // was set before it still belongs in the result.
                        ctx.LogInformation("Response processing was ended by qti-exit-response.");
                    }
                }
                ctx.CalculatedOutcomes.ToList().ForEach(outcomeIdentifier =>
                {
                    assessmentResult.PersistItemResultOutcome(assessmentItem.Identifier, outcomeIdentifier, ctx);
                });
            }
            return ctx.AssessmentResult;
        }

    }
}
