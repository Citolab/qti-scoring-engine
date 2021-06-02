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
using Citolab.QTI.ScoringEngine.ResponseProcessing.Interfaces;

namespace Citolab.QTI.ScoringEngine.ResponseProcessing
{
    internal class ResponseProcessorContext : IContextLogger
    {

        private readonly ILogger _logger;
        private bool _initOperators = false;
        public AssessmentResult AssessmentResult { get; }
        public AssessmentItem AssessmentItem { get; }
        public ItemResult ItemResult { get; set; }
        public Dictionary<string, IBooleanExpression> BooleanExpressions;
        public Dictionary<string, IBaseValueExpression> BaseValueExpressions;
        public Dictionary<string, ICustomOperator> CustomOperators = new Dictionary<string, ICustomOperator>();
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

        public IBaseValueExpression GetExpression(XElement element, ResponseProcessorContext context, bool logErrorIfNotFound = false)
        {
            if (BaseValueExpressions == null)
            {
                var type = typeof(IBaseValueExpression);
                var types = AppDomain.CurrentDomain.GetAssemblies()
                      .SelectMany(s => s.GetTypes())
                      .Where(p => type.IsAssignableFrom(p) && !p.IsInterface);
                var instances = types.Select(t => (IBaseValueExpression)Activator.CreateInstance(t));

                BaseValueExpressions = instances.ToDictionary(t => t.Name, t => t);
            }
            if (BaseValueExpressions.TryGetValue(element?.Name.LocalName, out var calculator))
            {
                context.LogInformation($"Processing {calculator.Name}");
                return calculator;
            }
            if (logErrorIfNotFound)
            {
                context.LogError($"Cannot find expression for tag-name:{element?.Name.LocalName}");
            }
            return null;
        }

        public ICustomOperator GetCustomOperator(XElement element, ResponseProcessorContext context, bool logErrorIfNotFound = false)
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

        public IBooleanExpression GetOperator(XElement element, ResponseProcessorContext context)
        {
            if (BooleanExpressions == null)
            {
                var type = typeof(IBooleanExpression);
                var types = AppDomain.CurrentDomain.GetAssemblies()
                      .SelectMany(s => s.GetTypes())
                      .Where(p => type.IsAssignableFrom(p) && !p.IsInterface);
                var instances = types.Select(t => (IBooleanExpression)Activator.CreateInstance(t));

                BooleanExpressions = instances.ToDictionary(t => t.Name, t => t);
            }
            if (BooleanExpressions.TryGetValue(element?.Name.LocalName, out var executor))
            {
                context.LogInformation($"Processing {executor.Name}");
                return executor;
            }
            context.LogError($"Cannot find expression for tag-name:{element?.Name.LocalName}");
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
