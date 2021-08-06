using Microsoft.Extensions.Logging;
using Citolab.QTI.ScoringEngine.Helpers;
using Citolab.QTI.ScoringEngine.Interfaces;
using Citolab.QTI.ScoringEngine.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;
using System.Globalization;
using Citolab.QTI.ScoringEngine.Expressions.GeneralExpressions;
using Citolab.QTI.ScoringEngine.Expressions.BaseValueExpression;

namespace Citolab.QTI.ScoringEngine.OutcomeProcessing
{
    // Before processing all variables/declarations are s
    internal class OutcomeProcessorContext : ProcessorContextBase
    {      
        public AssessmentTest AssessmentTest { get; }
        public TestResult TestResult { get; set; }

        public OutcomeProcessorContext(AssessmentResult assessmentResult, AssessmentTest assessmentTest, ILogger logger): base(logger, assessmentResult)
        {
            _sessionIdentifier = $"{ assessmentTest?.Identifier} - { assessmentResult?.SourcedId}";
            AssessmentTest = assessmentTest;
            // This will always be empty
            // but because there is not a clear seperation between the expressions that
            // can be used in outcome and responseprocessing the expressions share the same context.
            ResponseDeclarations = new Dictionary<string, ResponseDeclaration>();
            ResponseVariables = new Dictionary<string, ResponseVariable>();

            // these will be used.
            OutcomeDeclarations = assessmentTest.OutcomeDeclarations;
            if (!assessmentResult.TestResults.ContainsKey(assessmentTest.Identifier))
            {
                assessmentResult.AddTestResult(assessmentTest.Identifier);
            }
            TestResult = assessmentResult.TestResults[assessmentTest.Identifier];
            OutcomeVariables = TestResult.OutcomeVariables;
            CalculatedOutcomes = assessmentTest.CalculatedOutcomes;

            ResetOutcomes();
        }

        internal bool ItemResultExists(string itemIdentifier)
        {
            return AssessmentResult.ItemResults.ContainsKey(itemIdentifier);
        }

        internal BaseValue GetItemResultBaseValue(string itemIdentifier, string outcomeIdentifier, string weightIdentifier)
        {
            if (AssessmentResult.ItemResults.ContainsKey(itemIdentifier))
            {
                var itemResult = AssessmentResult.ItemResults[itemIdentifier];
                if (itemResult.OutcomeVariables.ContainsKey(outcomeIdentifier))
                {
                    var outcome = itemResult.OutcomeVariables[outcomeIdentifier];
                    var baseValue = outcome.ToBaseValue();
                    if (!string.IsNullOrEmpty(weightIdentifier))
                    {
                        if (AssessmentTest.AssessmentItemRefs.ContainsKey(itemIdentifier))
                        {
                            var itemRef = AssessmentTest.AssessmentItemRefs[itemIdentifier];
                            if (itemRef.Weights.ContainsKey(weightIdentifier))
                            {
                                var weight = itemRef.Weights[weightIdentifier];
                                if (baseValue.Value.TryParseFloat(out var result))
                                {
                                    baseValue.Value = (result * weight).ToString(CultureInfo.InvariantCulture);
                                }
                            }
                            else
                            {
                                LogError($"Cannot find weight: {weightIdentifier} for item: {itemIdentifier}");
                            }
                        }
                        else
                        {
                            LogError($"Cannot find itemRef: {itemIdentifier} to get weight");
                        }
                    }
                    return baseValue;
                }
                else
                {
                    LogInformation($"Cannot find outcome: {outcomeIdentifier} in itemResult {itemIdentifier}");
                    return 0.0F.ToBaseValue();
                }
            }
            else
            {
                LogInformation($"Cannot find itemResult: {itemIdentifier}");
                return 0.0F.ToBaseValue();
            }
        }

    }
}
