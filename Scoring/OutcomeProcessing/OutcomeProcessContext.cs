using Microsoft.Extensions.Logging;
using Citolab.QTI.ScoringEngine.Helper;
using Citolab.QTI.ScoringEngine.Interfaces;
using Citolab.QTI.ScoringEngine.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;

namespace Citolab.QTI.ScoringEngine.OutcomeProcessing
{
    // Before processing all variables/declarations are s
    public class OutcomeProcessContext
    {
        private readonly ILogger _logger;
        public AssessmentResult AssessmentResult { get; }
        public AssessmentTest AssessmentTest{ get; }

        public TestResult TestResult { get; set; }
        public OutcomeProcessContext(AssessmentResult assessmentResult, AssessmentTest assessmentTest, ILogger logger)
        {
            _logger = logger;
            AssessmentResult = assessmentResult;
            AssessmentTest = assessmentTest;
            if (!assessmentResult.TestResults.ContainsKey(assessmentTest.Identifier)) {
                assessmentResult.AddTestResult(assessmentTest.Identifier);
            }
            TestResult = assessmentResult.TestResults[assessmentTest.Identifier];
        }

        public void LogInformation(string value)
        {
            _logger.LogInformation($"{AssessmentTest.Identifier} - {AssessmentResult.SourceId}: {value}");
        }
        public void LogWarning(string value)
        {
            _logger.LogWarning($"{AssessmentTest.Identifier} - {AssessmentResult.SourceId}: {value}");
        }

        public void LogError(string value)
        {
            _logger.LogError($"{AssessmentTest.Identifier} - {AssessmentResult.SourceId}: {value}");
        }
    }
}
