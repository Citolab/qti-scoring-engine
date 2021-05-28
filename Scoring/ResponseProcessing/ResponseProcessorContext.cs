using Microsoft.Extensions.Logging;
using Citolab.QTI.Scoring.Const;
using Citolab.QTI.Scoring.Helper;
using Citolab.QTI.Scoring.Interfaces;
using Citolab.QTI.Scoring.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;
using Citolab.QTI.Scoring.ResponseProcessing.Interfaces;

namespace Citolab.QTI.Scoring.ResponseProcessing
{
    internal class ResponseProcessorContext : IContextLogger
    {

        private readonly ILogger _logger;
        private bool _initOperators = false;
        public AssessmentResult AssessmentResult { get; }
        public AssessmentItem AssessmentItem { get; }
        public ItemResult ItemResult { get; set; }
        public Dictionary<string, IResponseProcessingOperator> Operators;
        public Dictionary<string, IResponseProcessingExpression> Expressions;
        public Dictionary<string, ICustomOperator> CustomOperators;
        internal ResponseProcessorContext(ILogger logger, AssessmentResult assessmentResult, AssessmentItem assessmentItem, List<ICustomOperator> customOperators)
        {
            _logger = logger;
            AssessmentResult = assessmentResult;
            AssessmentItem = assessmentItem;
            if (AssessmentItem != null && AssessmentResult.ItemResults.ContainsKey(AssessmentItem.Identifier))
            {
                ItemResult = AssessmentResult.ItemResults[AssessmentItem.Identifier];
            }
            if (customOperators != null)
            {
                foreach(var customOperator in customOperators)
                {
                    CustomOperators.Add(customOperator.Definition, customOperator);
                }
            }
        }

        public IResponseProcessingExpression GetCalculator(XElement element, ResponseProcessorContext context, bool logErrorIfNotFound = false)
        {
            if (Expressions == null)
            {
                var type = typeof(IResponseProcessingExpression);
                var types = AppDomain.CurrentDomain.GetAssemblies()
                      .SelectMany(s => s.GetTypes())
                      .Where(p => type.IsAssignableFrom(p) && !p.IsInterface);
                var instances = types.Select(t => (IResponseProcessingExpression)Activator.CreateInstance(t));

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

        public ICustomOperator GetOperator(XElement element, ResponseProcessorContext context, bool logErrorIfNotFound = false)
        {
            if (_initOperators == false)
            {
                var type = typeof(ICustomOperator);
                var types = AppDomain.CurrentDomain.GetAssemblies()
                      .SelectMany(s => s.GetTypes())
                      .Where(p => type.IsAssignableFrom(p) && !p.IsInterface);
                var instances = types.Select(t => (ICustomOperator)Activator.CreateInstance(t));

                foreach(var customOperatorPair in instances.ToDictionary(t => t.Definition, t => t))
                {
                    if (!CustomOperators.ContainsKey(customOperatorPair.Key))
                    {
                        CustomOperators.Add(customOperatorPair.Key, customOperatorPair.Value);
                    }
                }
                _initOperators = true;
            }
            if (CustomOperators.TryGetValue(element?.GetAttributeValue("definition"), out var customOperator))
            {
                context.LogInformation($"Processing {customOperator.Definition}");
                return customOperator;
            }
            if (logErrorIfNotFound)
            {
                context.LogError($"Cannot find customOperator of type:{element?.GetAttributeValue("definition")}");
            }
            return null;
        }

        public IResponseProcessingOperator GetExecutor(XElement element, ResponseProcessorContext context)
        {
            if (Operators == null)
            {
                var type = typeof(IResponseProcessingOperator);
                var types = AppDomain.CurrentDomain.GetAssemblies()
                      .SelectMany(s => s.GetTypes())
                      .Where(p => type.IsAssignableFrom(p) && !p.IsInterface);
                var instances = types.Select(t => (IResponseProcessingOperator)Activator.CreateInstance(t));

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
        public void LogInformation(string value)
        {
            _logger.LogInformation($"{AssessmentItem?.Identifier} - {AssessmentResult?.SourcedId}: {value}");
        }
        public void LogWarning(string value)
        {
            _logger.LogWarning($"{AssessmentItem?.Identifier} - {AssessmentResult?.SourcedId}: {value}");
        }

        public void LogError(string value)
        {
            _logger.LogError($"{AssessmentItem?.Identifier} - {AssessmentResult?.SourcedId}: {value}");
        }
    }
}
