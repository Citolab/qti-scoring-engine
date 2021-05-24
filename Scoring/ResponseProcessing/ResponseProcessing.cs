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

namespace Citolab.QTI.ScoringEngine.ResponseProcessing
{
    public class ResponseProcessing
    {
        public AssessmentResult Process(AssessmentItem assessmentItem, AssessmentResult assessmentResult, ILogger logger)
        {
            var context = new ResponseProcessingContext(logger, assessmentResult, assessmentItem);
            // Reset all values that are recalculated;
            context.ItemResult.OutcomeVariables.Where(o =>
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
                    var executor = ExecuteFactory.GetExecutor(responseProcessingChild, context);
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
