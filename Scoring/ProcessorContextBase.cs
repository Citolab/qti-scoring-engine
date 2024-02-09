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
    internal abstract class ProcessorContextBase : IProcessingContext
    {
        protected readonly ILogger _logger;
        protected string _sessionIdentifier;

        public AssessmentResult AssessmentResult { get; }
        public Dictionary<string, ResponseDeclaration> ResponseDeclarations { get; set; }
        public Dictionary<string, OutcomeDeclaration> OutcomeDeclarations { get; set; }
        public Dictionary<string, ResponseVariable> ResponseVariables { get; set; }
        public Dictionary<string, OutcomeVariable> OutcomeVariables { get; set; }
        public HashSet<string> CalculatedOutcomes { get; set; }


        public ProcessorContextBase(ILogger logger, AssessmentResult assessmentResult)
        {
            _logger = logger;
            AssessmentResult = assessmentResult;
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
