using System;
using System.Linq;
using System.Collections.Generic;
using Citolab.QTI.ScoringEngine.Interfaces;
using System.Xml.Linq;
using Microsoft.Extensions.Logging;
using Citolab.QTI.ScoringEngine.Model;
using Citolab.QTI.ScoringEngine.Const;

namespace Citolab.QTI.ScoringEngine.Helpers
{
    internal class ExpressionFactory : IExpressionFactory
    {
        private readonly ILogger _logger;
        private readonly Dictionary<string, ICustomOperator> _customOperators;

        public ExpressionFactory(Dictionary<string, ICustomOperator> addedCustomOperators, ILogger logger)
        {
            _logger = logger;
            // Each factory gets its own registry: the built-in operators overlaid with the
            // caller's. Writing the added ones into Mappings.CustomOperators used to leak them
            // into every other ScoringEngine in the process, for the life of the process.
            _customOperators = new Dictionary<string, ICustomOperator>(Mappings.CustomOperators);
            if (addedCustomOperators != null)
            {
                foreach (var addedCustomOperator in addedCustomOperators)
                {
                    // an added operator overrides a built-in one with the same key
                    _customOperators[addedCustomOperator.Key] = addedCustomOperator.Value;
                }
            }
        }

        public IConditionExpression GetConditionExpression(XElement qtiElement, bool logErrorIfNotFound)
        {
            if (Mappings.ConditionalExpressions.TryGetValue(qtiElement.Name.LocalName, out var condinalExpressionType))
            {
                var conditionExpression = (IConditionExpression)Activator.CreateInstance(condinalExpressionType);
                _logger.LogInformation($"Setting up {qtiElement.Name.LocalName}");
                conditionExpression.Init(qtiElement, this);
                return conditionExpression;
            }
            else
            {
                if (logErrorIfNotFound)
                {
                    _logger.LogError($"Cannot find condition expression with key: {qtiElement.Name.LocalName}");
                }
                return null;
            }
        }

        public ICustomOperator GetCustomOperator(string defintion)
        {
            if (_customOperators.TryGetValue(defintion, out var customOperator))
            {
                return customOperator;
            }
            _logger.LogError($"Cannot find customOperator with key: {defintion}");
            return null;
        }

        public IValueExpression GetValueExpression(XElement qtiElement, bool logErrorIfNotFound)
        {
            if (Mappings.ValueExpressions.TryGetValue(qtiElement.Name.LocalName, out var valueExpressionType))
            {
                var valueExpression = (IValueExpression)Activator.CreateInstance(valueExpressionType);
                _logger.LogInformation($"Setting up {qtiElement.Name.LocalName}");
                valueExpression.Init(qtiElement, this);
                return valueExpression;
            }
            else
            {
                if (logErrorIfNotFound)
                {
                    _logger.LogError($"Cannot find value expression with key: {qtiElement.Name.LocalName}");
                }
                return null;
            }
        }
    }
}
