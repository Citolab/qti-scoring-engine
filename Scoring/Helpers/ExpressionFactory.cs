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


        public ExpressionFactory(Dictionary<string, ICustomOperator> addedCustomOperators, ILogger logger)
        {
            _logger = logger;
            if (addedCustomOperators != null)
            {
                foreach (var addedCustomOperator in addedCustomOperators)
                {
                    if (Mappings.CustomOperators.ContainsKey(addedCustomOperator.Key))
                    {
                        // override with added value
                        Mappings.CustomOperators[addedCustomOperator.Key] = addedCustomOperator.Value;
                    }
                    else
                    {
                        Mappings.CustomOperators.Add(addedCustomOperator.Key, addedCustomOperator.Value);
                    }
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
            if (Mappings.CustomOperators.ContainsKey(defintion))
            {
                return Mappings.CustomOperators[defintion];
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
