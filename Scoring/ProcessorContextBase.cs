using Citolab.QTI.ScoringEngine.Expressions.BaseValueExpression;
using Citolab.QTI.ScoringEngine.Helpers;
using Citolab.QTI.ScoringEngine.Interfaces;
using Citolab.QTI.ScoringEngine.Model;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
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
        public Dictionary<string, BaseValue> ContextVariables { get; set; } = new Dictionary<string, BaseValue>();
        public HashSet<string> CalculatedOutcomes { get; set; }

        internal const string QtiContextIdentifier = "QTI_CONTEXT";

        public ProcessorContextBase(ILogger logger, AssessmentResult assessmentResult)
        {
            _logger = logger;
            AssessmentResult = assessmentResult;
            InitQtiContext();
        }

        /// <summary>
        /// QTI_CONTEXT is the record the delivery engine is expected to provide. What is known
        /// of it here comes from the assessmentResult: the candidate it is of and the test it
        /// belongs to. Fields that are unknown are left out, so qti-field-value returns NULL
        /// for them rather than an empty string.
        /// </summary>
        private void InitQtiContext()
        {
            var fields = new Dictionary<string, BaseValue>();
            SetField(fields, "candidateIdentifier", AssessmentResult?.SourcedId);
            SetField(fields, "testIdentifier", AssessmentResult?.TestResults?.Keys.FirstOrDefault());
            var contextElement = AssessmentResult?.FindElementByName("context");
            SetField(fields, "environmentIdentifier",
                contextElement?.GetAttributeValue("environmentIdentifier", "environment-identifier"));
            ContextVariables[QtiContextIdentifier] = new BaseValue
            {
                Identifier = QtiContextIdentifier,
                BaseType = BaseType.Identifier,
                Cardinality = Cardinality.Record,
                Fields = fields
            };
        }

        /// <summary>
        /// Adds or replaces a field of QTI_CONTEXT. An empty value removes the field, because a
        /// field that is not there is NULL and a field that is there but empty is not.
        /// </summary>
        protected void SetQtiContextField(string fieldIdentifier, string value)
        {
            SetField(ContextVariables[QtiContextIdentifier].Fields, fieldIdentifier, value);
        }

        private static void SetField(Dictionary<string, BaseValue> fields, string fieldIdentifier, string value)
        {
            if (string.IsNullOrEmpty(value))
            {
                fields.Remove(fieldIdentifier);
                return;
            }
            fields[fieldIdentifier] = new BaseValue
            {
                Identifier = fieldIdentifier,
                BaseType = BaseType.Identifier,
                Cardinality = Cardinality.Single,
                Value = value
            };
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
