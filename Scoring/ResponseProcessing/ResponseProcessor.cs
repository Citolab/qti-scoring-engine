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
        internal static AssessmentResult Process(AssessmentItem assessmentItem, AssessmentResult assessmentResult, ILogger logger, List<ICustomOperator> customOperators = null)
        {
            var context = new ResponseProcessorContext(logger, assessmentResult, assessmentItem, customOperators);
            // Skip processing when there is no itemResult
            var containsItemResult = context.ItemResult != null;

            // Reset all values that are recalculated;
            if (containsItemResult)
            {
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
                }
                else
                {
                    context.LogInformation("No responseProcessing found");
                }
            }
            assessmentItem.CalculatedOutcomes.ToList().ForEach(outcomeIdentifier =>
            {
                assessmentResult.PersistItemResultOutcome(assessmentItem.Identifier, outcomeIdentifier, context);
            });
            return context.AssessmentResult;
        }

    }
}
