using Microsoft.Extensions.Logging;
using Citolab.QTI.Scoring.ResponseProcessing;
using Citolab.QTI.Scoring.Helper;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;
using Citolab.QTI.Scoring.Model;

namespace Citolab.QTI.Scoring.ResponseProcessing
{
    internal static class ResponseProcessor
    {
        internal static AssessmentResult Process(AssessmentItem assessmentItem, AssessmentResult assessmentResult, ILogger logger)
        {
            var context = new ResponseProcessorContext(logger, assessmentResult, assessmentItem);
            // Skip processing when there is no itemResult
            if (context.ItemResult == null)
            {
                return assessmentResult;
            }
            // Reset all values that are recalculated;
            context.ItemResult?.OutcomeVariables?.Where(o =>
            {
                return assessmentItem.CalculatedOutcomes.Contains(o.Key);
            }).ToList()
            .ForEach(o =>
            {
                o.Value.Value = 0;
            });
            if (assessmentItem.ResponseProcessingElement != null)
            {
                foreach (var responseProcessingChild in assessmentItem.ResponseProcessingElement.Elements())
                {
                    var executor = context.GetExecutor(responseProcessingChild, context);
                    executor?.Execute(responseProcessingChild, context);
                }
                assessmentItem.CalculatedOutcomes.ToList().ForEach(outcomeIdentifier =>
                {
                    assessmentResult.PersistItemResultOutcome(assessmentItem.Identifier, outcomeIdentifier);
                });
            }
            else
            {
                context.LogInformation("No responseProcessing found");
            }
            return context.AssessmentResult;
        }

    }
}
