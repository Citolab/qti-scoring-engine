using Microsoft.Extensions.Logging;
using Citolab.QTI.ScoringEngine.Const;
using Citolab.QTI.ScoringEngine.Helpers;
using Citolab.QTI.ScoringEngine.Interfaces;
using Citolab.QTI.ScoringEngine.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;
using Citolab.QTI.ScoringEngine.Expressions.GeneralExpressions;

namespace Citolab.QTI.ScoringEngine.ResponseProcessing
{
    internal class ResponseProcessorContext :  ProcessorContextBase
    {
        public AssessmentItem AssessmentItem { get; }
        public ItemResult ItemResult { get; set; }
            internal ResponseProcessorContext(ILogger logger, AssessmentResult assessmentResult, AssessmentItem assessmentItem): base(logger, assessmentResult)
        {
            _sessionIdentifier = $"{AssessmentItem?.Identifier} - {AssessmentResult?.SourcedId}";
            AssessmentItem = assessmentItem;
            if (AssessmentItem != null && AssessmentResult.ItemResults.ContainsKey(AssessmentItem.Identifier))
            {
                ItemResult = AssessmentResult.ItemResults[AssessmentItem.Identifier];

                ResponseDeclarations = AssessmentItem.ResponseDeclarations;
                OutcomeDeclarations = AssessmentItem.OutcomeDeclarations;
                CalculatedOutcomes = AssessmentItem.CalculatedOutcomes;

                OutcomeVariables = ItemResult.OutcomeVariables;
                ResponseVariables = ItemResult.ResponseVariables;
            } else
            {
                LogInformation("Cannot find item result");
            }
            ResetOutcomes();

        }




    }
}
