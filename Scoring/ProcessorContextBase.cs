using Citolab.QTI.ScoringEngine.Expressions.BaseValueExpression;
using Citolab.QTI.ScoringEngine.Helpers;
using Citolab.QTI.ScoringEngine.Interfaces;
using Citolab.QTI.ScoringEngine.Model;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Text;
using System.Xml.Linq;

namespace Citolab.QTI.ScoringEngine
{
    internal abstract class ProcessorContextBase: IProcessingContext
    {
        protected readonly ILogger _logger;
        private readonly List<ICustomOperator> _addedCustomOperators;
        protected string _sessionIdentifier;

        public AssessmentResult AssessmentResult { get; }
        public Dictionary<string, ResponseDeclaration> ResponseDeclarations { get; set; }
        public Dictionary<string, OutcomeDeclaration> OutcomeDeclarations { get; set; }
        public Dictionary<string, ResponseVariable> ResponseVariables { get; set; }
        public Dictionary<string, OutcomeVariable> OutcomeVariables { get; set; }
        public HashSet<string> CalculatedOutcomes { get; set; }
        public Dictionary<string, IExpression> ValueExpressions { get; set; }
        public Dictionary<string, IConditionExpression> ConditionExpressions { get; set; }
        public ProcessorContextBase(ILogger logger, AssessmentResult assessmentResult, List<ICustomOperator> addedCustomOperators)
        {
            _logger = logger;
            AssessmentResult = assessmentResult;
            _addedCustomOperators = addedCustomOperators;
        }


        protected void ResetOutcomes()
        {
            if (CalculatedOutcomes != null)
            {
                foreach (var calculatedOutcome in CalculatedOutcomes)
                {
                    if (OutcomeVariables.ContainsKey(calculatedOutcome))
                    {
                        OutcomeVariables.Remove(calculatedOutcome);
                    }
                    if (!OutcomeDeclarations.ContainsKey(calculatedOutcome))
                    {
                        LogWarning($"outcome declaration: {calculatedOutcome} does not exist. Adding value and continue");
                        // this is tricky but probably is ok in most cases. Therefor log warning.
                        OutcomeDeclarations.Add(calculatedOutcome, new OutcomeDeclaration
                        {
                            Identifier = calculatedOutcome,
                            BaseType = BaseType.Float,
                            Cardinality = Cardinality.Single
                        });
                    }
                    var outcome = OutcomeDeclarations[calculatedOutcome];
                    var variable = outcome.ToVariable();
                    OutcomeVariables.Add(variable.Identifier, variable);
                }
            }
        }

        public BaseValue GetValue(XElement qtiElement)
        {
            var expression = GetValueExpression(qtiElement, false);
            return expression?.Apply(qtiElement, this);
        }

        private IExpression GetValueExpression(XElement qtiElement, bool logErrorIfNotFound)
        {
            var tagName = qtiElement.Name.LocalName;
            if (tagName == "qti-custom-operator")
            {
                tagName = $"qti-custom-operator-{qtiElement.GetAttributeValue("definition")}";
            }
            if (ValueExpressions.TryGetValue(tagName, out var valueExpression))
            {
                LogInformation($"Processing {valueExpression.Name}");
                return valueExpression;
            }
            if (logErrorIfNotFound)
            {
                LogError($"Cannot find expression for tag-name:{tagName}");
            }
            return null;
        }

        private IConditionExpression GetConditionExpression(XElement qtiElement, bool logErrorIfNotFound)
        {
            var tagName = qtiElement.Name.LocalName;
            if (ConditionExpressions.TryGetValue(tagName, out var conditionExpression))
            {
                LogInformation($"Processing {conditionExpression.Name}");
                return conditionExpression;
            }
            if (logErrorIfNotFound)
            {
                LogError($"Cannot find expression for tag-name:{tagName}");
            }
            return null;
        }

        public void Execute(XElement qtiElement)
        {
            var expression = GetConditionExpression(qtiElement, true);
            expression.Execute(qtiElement, this);
        }

        public bool ExpressionSupported(XElement qtiElement)
        {
            var expression = GetConditionExpression(qtiElement, true);
            return expression != null;
        }
        public bool CheckCondition(XElement qtiElement)
        {
            var expression = GetConditionExpression(qtiElement, true);
            return expression == null ? false : expression.Execute(qtiElement, this);
        }
        protected void SetupExpressions(Type typeToExcluded)
        {
            if (ValueExpressions == null)
            {
                ValueExpressions = Helper.GetExpressions(new List<Type>
                    {
                        typeToExcluded,
                        typeof(IOperatorWrapper)
                    }, _addedCustomOperators);
 
            }
        }


        public void LogInformation(string value)
        {
            _logger.LogInformation($"{_sessionIdentifier}: {value}");
        }
        public void LogWarning(string value)
        {
            _logger.LogWarning($"{_sessionIdentifier}: {value}");
        }

        public void LogError(string value)
        {
            _logger.LogError($"{_sessionIdentifier}: {value}");
        }

    }
}
