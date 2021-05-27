using Microsoft.Extensions.Logging;
using Citolab.QTI.Scoring.Helper;
using Citolab.QTI.Scoring.Interfaces;
using Citolab.QTI.Scoring.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;

namespace Citolab.QTI.Scoring.OutcomeProcessing
{
    // Before processing all variables/declarations are s
    internal class OutcomeProcessorContext
    {
        private readonly ILogger _logger;
        public AssessmentResult AssessmentResult { get; }
        public AssessmentTest AssessmentTest{ get; }

        public TestResult TestResult { get; set; }
        public Dictionary<string, ICalculateOutcomeProcessing> Calculators;
        public Dictionary<string, IExecuteOutcomeProcessing> Executors;

        public OutcomeProcessorContext(AssessmentResult assessmentResult, AssessmentTest assessmentTest, ILogger logger)
        {
            _logger = logger;
            AssessmentResult = assessmentResult;
            AssessmentTest = assessmentTest;
            if (!assessmentResult.TestResults.ContainsKey(assessmentTest.Identifier)) {
                assessmentResult.AddTestResult(assessmentTest.Identifier);
            }
            TestResult = assessmentResult.TestResults[assessmentTest.Identifier];
        }

        public IExecuteOutcomeProcessing GetExecutor(XElement element, OutcomeProcessorContext context)
        {
            if (Executors == null)
            {
                var type = typeof(IExecuteOutcomeProcessing);
                var types = AppDomain.CurrentDomain.GetAssemblies()
                      .SelectMany(s => s.GetTypes())
                      .Where(p => type.IsAssignableFrom(p) && !p.IsInterface);
                var instances = types.Select(t => (IExecuteOutcomeProcessing)Activator.CreateInstance(t));

                Executors = instances.ToDictionary(t => t.Name, t => t);
            }
            if (Executors.TryGetValue(element?.Name.LocalName, out var executor))
            {
                context.LogInformation($"Processing {executor.Name}");
                return executor;
            }
            context.LogError($"Cannot find executor for tag-name:{element?.Name.LocalName}");
            return null;
        }

        public ICalculateOutcomeProcessing GetCalculator(XElement element, OutcomeProcessorContext context, bool logErrorIfNotFound = false)
        {
            if (Calculators == null)
            {
                var type = typeof(ICalculateOutcomeProcessing);
                var types = AppDomain.CurrentDomain.GetAssemblies()
                      .SelectMany(s => s.GetTypes())
                      .Where(p => type.IsAssignableFrom(p) && !p.IsInterface);
                var instances = types.Select(t => (ICalculateOutcomeProcessing)Activator.CreateInstance(t));

                Calculators = instances.ToDictionary(t => t.Name, t => t);
            }
            if (Calculators.TryGetValue(element?.Name.LocalName, out var calculator))
            {
                context.LogInformation($"Processing {calculator.Name}");
                return calculator;
            }
            if (logErrorIfNotFound)
            {
                context.LogError($"Cannot find calculator for tag-name:{element?.Name.LocalName}");
            }
            return null;
        }

        public void LogInformation(string value)
        {
            _logger.LogInformation($"{AssessmentTest?.Identifier} - {AssessmentResult?.SourcedId}: {value}");
        }
        public void LogWarning(string value)
        {
            _logger.LogWarning($"{AssessmentTest?.Identifier} - {AssessmentResult?.SourcedId}: {value}");
        }

        public void LogError(string value)
        {
            _logger.LogError($"{AssessmentTest?.Identifier} - {AssessmentResult?.SourcedId}: {value}");
        }
    }
}
