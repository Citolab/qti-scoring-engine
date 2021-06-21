using System;
using System.Linq;
using System.Collections.Generic;
using System.Text;
using Citolab.QTI.ScoringEngine.Interfaces;
using Citolab.QTI.ScoringEngine.Expressions.GeneralExpressions;
using Citolab.QTI.ScoringEngine.Expressions.BaseValueExpression;
using System.Xml.Linq;
using Microsoft.Extensions.Logging;

namespace Citolab.QTI.ScoringEngine.Helpers
{
    internal class ExpressionFactory
    {
        private readonly List<ICustomOperator> _addedCustomOperators;
        private Dictionary<string, IExpression> ValueExpressions { get; set; }
        private Dictionary<string, IConditionExpression> ConditionExpressions { get; set; }
        private readonly ILogger _logger;
        public ExpressionFactory(List<ICustomOperator> addedCustomOperators, ILogger logger)
        {
            _logger = logger;
            _addedCustomOperators = addedCustomOperators ?? new List<ICustomOperator>() ;
            SetupForResponseProcession();
        }

        public IExpression GetValueExpression(XElement qtiElement, bool logErrorIfNotFound)
        {
            var tagName = qtiElement.Name.LocalName;
            if (tagName == "qti-custom-operator")
            {
                tagName = $"qti-custom-operator-{qtiElement.GetAttributeValue("definition")}";
            }
            if (ValueExpressions.TryGetValue(tagName, out var valueExpression))
            {
                _logger.LogInformation($"Processing {valueExpression.Name}");
                return valueExpression;
            }
            if (logErrorIfNotFound)
            {
                _logger.LogError($"Cannot find expression for tag-name:{tagName}");
            }
            return null;
        }

        private IConditionExpression GetConditionExpression(XElement qtiElement, bool logErrorIfNotFound)
        {
            var tagName = qtiElement.Name.LocalName;
            if (ConditionExpressions.TryGetValue(tagName, out var conditionExpression))
            {
                _logger.LogInformation($"Processing {conditionExpression.Name}");
                return conditionExpression;
            }
            if (logErrorIfNotFound)
            {
                _logger.LogError($"Cannot find expression for tag-name:{tagName}");
            }
            return null;
        }

        private void SetupForResponseProcession()
        {
            ConditionExpressions = GetConditionExpressions(new List<Type> { typeof(IOutcomeProcessingConditionExpression) });
            SetupExpressions(typeof(IOutcomeProcessingExpression));
        }

       private static Dictionary<string, IConditionExpression> GetConditionExpressions(List<Type> excludedInterfaces)
        {
            var assemblies = AppDomain.CurrentDomain.GetAssemblies();
            return assemblies
                  .SelectMany(s => s.GetTypes())
                 .Where(p =>
                 {
                     var interfaces = p.GetInterfaces();
                     if (interfaces != null)
                     {
                         return !p.IsInterface && interfaces.Contains(typeof(IConditionExpression)) &&
                             !interfaces.Intersect(excludedInterfaces).Any();
                     }
                     return false;
                 })
                  .Select(t => (IConditionExpression)Activator.CreateInstance(t))
                  .ToDictionary(e => e.Name, e => e);
        }

        // Helper for inside, handles circle shapes.

        internal static Dictionary<string, IExpression> GetExpressions(List<Type> excludedInterfaces, List<ICustomOperator> addedCustomOperators)
        {
            var assemblies = AppDomain.CurrentDomain.GetAssemblies();
            // get all classes that implement IExpression except for excluded like: 
            // No ResponseProcessingExpression in Outcome processing and the other way around.
            var expressionDictionary = assemblies
                  .SelectMany(s => s.GetTypes())
                  .Where(p =>
                  {
                      var interfaces = p.GetInterfaces();
                      if (interfaces != null)
                      {
                          return !p.IsInterface && interfaces.Contains(typeof(IExpression)) &&
                              !interfaces.Intersect(excludedInterfaces).Any();
                      }
                      return false;
                  })
                  .Select(t => (IExpression)Activator.CreateInstance(t))
                  .ToDictionary(expression => expression.Name, expression => expression);
            var operatorExpressions = assemblies
                  .SelectMany(s => s.GetTypes())
                  .Where(p => typeof(IOperator).IsAssignableFrom(p) && !p.IsInterface)
                    .Select(t => (IOperator)Activator.CreateInstance(t))
                    .Select(op => new OperatorWrapper(op, op.Name));

            foreach (var operatorExpression in operatorExpressions)
            {
                if (expressionDictionary.ContainsKey(operatorExpression.Name))
                    expressionDictionary[operatorExpression.Name] = operatorExpression;
                else
                    expressionDictionary.Add(operatorExpression.Name, operatorExpression);
            }

            if (addedCustomOperators == null)
            {
                addedCustomOperators = new List<ICustomOperator>();
            }

            var customOperatorExpressions = assemblies
                  .SelectMany(s => s.GetTypes())
                  .Where(p => typeof(ICustomOperator).IsAssignableFrom(p) && !p.IsInterface)
                  .Select(t => (ICustomOperator)Activator.CreateInstance(t))
                  .Concat(addedCustomOperators)
                  .Select(op => new OperatorWrapper(op, $"qti-custom-operator-{op.Definition}"));

            foreach (var operatorExpression in customOperatorExpressions)
            {
                if (expressionDictionary.ContainsKey(operatorExpression.Name))
                    expressionDictionary[operatorExpression.Name] = operatorExpression;
                else
                    expressionDictionary.Add(operatorExpression.Name, operatorExpression);
            }
            return expressionDictionary;
        }

        protected void SetupExpressions(Type typeToExcluded)
        {
            if (ValueExpressions == null)
            {
                ValueExpressions = GetExpressions(new List<Type>
                    {
                        typeToExcluded,
                        typeof(IOperatorWrapper)
                    }, _addedCustomOperators);

            }
        }
    }
}
