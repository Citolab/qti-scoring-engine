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
    internal class OutcomeProcessorContext : IContextLogger
    {
        private readonly ILogger _logger;
        public AssessmentResult AssessmentResult { get; }
        public AssessmentTest AssessmentTest{ get; }

        public TestResult TestResult { get; set; }
        public Dictionary<string, IOutcomeBaseValueExpression> BaseValueExpressions;
        public Dictionary<string, IOutcomeBooleanExpression> BooleanExpressions;

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

        public IOutcomeBooleanExpression GetOperator(XElement element, OutcomeProcessorContext context)
        {
            if (BooleanExpressions == null)
            {
                var type = typeof(IOutcomeBooleanExpression);
                var types = AppDomain.CurrentDomain.GetAssemblies()
                      .SelectMany(s => s.GetTypes())
                      .Where(p => type.IsAssignableFrom(p) && !p.IsInterface);
                var instances = types.Select(t => (IOutcomeBooleanExpression)Activator.CreateInstance(t));

                BooleanExpressions = instances.ToDictionary(t => t.Name, t => t);
            }
            if (BooleanExpressions.TryGetValue(element?.Name.LocalName, out var executor))
            {
                context.LogInformation($"Processing {executor.Name}");
                return executor;
            }
            context.LogError($"Cannot find executor for tag-name:{element?.Name.LocalName}");
            return null;
        }

        public IOutcomeBaseValueExpression GetExpression(XElement element, OutcomeProcessorContext context, bool logErrorIfNotFound = false)
        {
            if (BaseValueExpressions == null)
            {
                var type = typeof(IOutcomeBaseValueExpression);
                var types = AppDomain.CurrentDomain.GetAssemblies()
                      .SelectMany(s => s.GetTypes())
                      .Where(p => type.IsAssignableFrom(p) && !p.IsInterface);
                var instances = types.Select(t => (IOutcomeBaseValueExpression)Activator.CreateInstance(t));

                BaseValueExpressions = instances.ToDictionary(t => t.Name, t => t);
            }
            if (BaseValueExpressions.TryGetValue(element?.Name.LocalName, out var calculator))
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
