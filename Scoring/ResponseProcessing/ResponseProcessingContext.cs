using Microsoft.Extensions.Logging;
using Citolab.QTI.ScoringEngine.Const;
using Citolab.QTI.ScoringEngine.Helper;
using Citolab.QTI.ScoringEngine.Interfaces;
using Citolab.QTI.ScoringEngine.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;

namespace Citolab.QTI.ScoringEngine.ResponseProcessing
{
    public class ResponseProcessingContext
    {
        private readonly ILogger _logger;
        public AssessmentResult AssessmentResult { get; }
        public AssessmentItem AssessmentItem { get; }
        public ItemResult ItemResult { get; set; }

        public ResponseProcessingContext(ILogger logger, AssessmentResult assessmentResult, AssessmentItem assessmentItem)
        {
            _logger = logger;
            AssessmentResult = assessmentResult;
            AssessmentItem = assessmentItem;
            // Without itemResult there is no response, so skip procesing
            if (AssessmentResult.ItemResults.ContainsKey(AssessmentItem.Identifier))
            {
                ItemResult = AssessmentResult.ItemResults[AssessmentItem.Identifier];
            }
            else
            {
                LogWarning("Item result not found. Skipping ResponseProcessing");
            }

        }
        public void LogInformation(string value)
        {
            _logger.LogInformation($"{AssessmentItem.Identifier} - {AssessmentResult.SourceId}: {value}");
        }
        public void LogWarning(string value)
        {
            _logger.LogWarning($"{AssessmentItem.Identifier} - {AssessmentResult.SourceId}: {value}");
        }

        public void LogError(string value)
        {
            _logger.LogError($"{AssessmentItem.Identifier} - {AssessmentResult.SourceId}: {value}");
        }
    }
}
