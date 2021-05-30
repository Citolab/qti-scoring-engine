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
    internal class OutcomeProcessorContext : IContextLogger
    {
        private readonly ILogger _logger;
        public AssessmentResult AssessmentResult { get; }
        public AssessmentTest AssessmentTest{ get; }

        public TestResult TestResult { get; set; }
        public Dictionary<string, IOutcomeProcessingExpression> Expressions;
        public Dictionary<string, IOutcomeProcessingOperator> Operators;

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

        public IOutcomeProcessingOperator GetOperator(XElement element, OutcomeProcessorContext context)
        {
            if (Operators == null)
            {
                var type = typeof(IOutcomeProcessingOperator);
                var types = AppDomain.CurrentDomain.GetAssemblies()
                      .SelectMany(s => s.GetTypes())
                      .Where(p => type.IsAssignableFrom(p) && !p.IsInterface);
                var instances = types.Select(t => (IOutcomeProcessingOperator)Activator.CreateInstance(t));

                Operators = instances.ToDictionary(t => t.Name, t => t);
            }
            if (Operators.TryGetValue(element?.Name.LocalName, out var executor))
            {
                context.LogInformation($"Processing {executor.Name}");
                return executor;
            }
            context.LogError($"Cannot find executor for tag-name:{element?.Name.LocalName}");
            return null;
        }

        public IOutcomeProcessingExpression GetExpression(XElement element, OutcomeProcessorContext context, bool logErrorIfNotFound = false)
        {
            if (Expressions == null)
            {
                var type = typeof(IOutcomeProcessingExpression);
                var types = AppDomain.CurrentDomain.GetAssemblies()
                      .SelectMany(s => s.GetTypes())
                      .Where(p => type.IsAssignableFrom(p) && !p.IsInterface);
                var instances = types.Select(t => (IOutcomeProcessingExpression)Activator.CreateInstance(t));

                Expressions = instances.ToDictionary(t => t.Name, t => t);
            }
            if (Expressions.TryGetValue(element?.Name.LocalName, out var calculator))
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
