using Microsoft.Extensions.Logging;
using Citolab.QTI.ScoringEngine.ResponseProcessing;
using Citolab.QTI.ScoringEngine.Helper;
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
        internal static AssessmentResult Process(AssessmentItem assessmentItem, AssessmentResult assessmentResult,  ILogger logger, List<ICustomOperator> customOperators = null)
        {
            var context = new ResponseProcessorContext(logger, assessmentResult, assessmentItem, customOperators);
            // Skip processing when there is no itemResult
            if (context.ItemResult == null)
            {
                context.LogWarning("Item result not found. Skipping ResponseProcessing");
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
                    var qtiOperator = context.GetOperator(responseProcessingChild, context);
                    qtiOperator?.Execute(responseProcessingChild, context);
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
