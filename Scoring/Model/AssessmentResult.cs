using Microsoft.Extensions.Logging;
using Citolab.QTI.ScoringEngine.Const;
using Citolab.QTI.ScoringEngine.Helpers;
using Citolab.QTI.ScoringEngine.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;
using Citolab.QTI.ScoringEngine.ResponseProcessing;

namespace Citolab.QTI.ScoringEngine.Model
{
    internal class AssessmentResult : XDocument
    {
        public Dictionary<string, TestResult> TestResults = new Dictionary<string, TestResult>();
        public Dictionary<string, ItemResult> ItemResults = new Dictionary<string, ItemResult>();

        public string SourcedId { get; set; }

        private readonly ILogger _logger;
        public AssessmentResult(ILogger logger, XDocument assessmentResult) : base(assessmentResult)
        {
            _logger = logger;
            SourcedId = this.FindElementByName("context")?.GetAttributeValue("sourcedId");
            InitItemResults();
            InitTestResults();
        }

        public void InitItemResults()
        {
            ItemResults = Root
              .FindElementsByName("itemResult").Select(itemResultElement =>
              {
                  var itemIdentifier = itemResultElement.Identifier();
                  return GetResult<ItemResult>(itemResultElement);
              }).ToDictionary(itemResult => itemResult.Identifier, itemResult => itemResult);
        }
        public void InitTestResults()
        {
            TestResults = Root
            .FindElementsByName("testResult").Select(testResultElement =>
            {
                return GetResult<TestResult>(testResultElement);
            }).ToDictionary(itemResult => itemResult.Identifier, itemResult => itemResult);
        }

        public void AddItemResult(string itemIdentifier)
        {
            var itemResult = new ItemResult
            {
                Identifier = itemIdentifier,
                OutcomeVariables = new Dictionary<string, OutcomeVariable>(),
                ResponseVariables = new Dictionary<string, ResponseVariable>()
            };
            ItemResults.Add(itemIdentifier, itemResult);
            Root.Add(itemResult.ToElement().AddDefaultNamespace(Root.GetDefaultNamespace()));
        }

        public void AddTestResult(string testIdentifier)
        {
            var testResult = new TestResult
            {
                Identifier = testIdentifier,
                OutcomeVariables = new Dictionary<string, OutcomeVariable>(),
                ResponseVariables = new Dictionary<string, ResponseVariable>()
            };
            TestResults.Add(testIdentifier, testResult);
            Root.Add(testResult.ToElement().AddDefaultNamespace(Root.GetDefaultNamespace()));
        }

        public void PersistItemResultOutcome(string itemIdentifier, string outcomeIdentifier, ResponseProcessorContext context)
        {
            // add items result if there is no result yet.
            if (!ItemResults.ContainsKey(itemIdentifier))
            {
                AddItemResult(itemIdentifier);
            }
            var itemResultElement = this
                .FindElementsByElementAndAttributeValue("itemResult", "identifier", itemIdentifier)
                .FirstOrDefault();
            // if there is no outcome for this variable then add one.
            var outcomeExists = ItemResults[itemIdentifier].OutcomeVariables.ContainsKey(outcomeIdentifier);
            if (!outcomeExists)
            {
                var outcomeDeclaration = context.AssessmentItem.OutcomeDeclarations[outcomeIdentifier];
                var newOutcomeVariable = outcomeDeclaration.ToVariable();
                newOutcomeVariable.Value = string.IsNullOrWhiteSpace(outcomeDeclaration.DefaultValue?.ToString())
                    ? "0" : outcomeDeclaration.DefaultValue?.ToString();
                ItemResults[itemIdentifier].OutcomeVariables.Add(outcomeIdentifier, newOutcomeVariable);
                itemResultElement.Add(newOutcomeVariable.ToElement().AddDefaultNamespace(Root.GetDefaultNamespace()));
            }
            else
            {
                var outcome = ItemResults[itemIdentifier].OutcomeVariables[outcomeIdentifier];
                var outcomeVariable = itemResultElement?
                               .FindElementsByElementAndAttributeValue("outcomeVariable", "identifier", outcome.Identifier)
                               .FirstOrDefault();
                if (outcomeVariable == null)
                {
                    outcomeVariable = outcome.ToElement().AddDefaultNamespace(Root.GetDefaultNamespace());
                    itemResultElement.Add(outcomeVariable.AddDefaultNamespace(Root.GetDefaultNamespace()));
                } else
                {
                    outcomeVariable.RemoveNodes();
                    foreach(var outcomeChild in outcome.ToElement().Elements())
                    {
                        outcomeVariable.Add(outcomeChild);
                    }
                  
                }
                //if (outcome.Value != null)
                //{
                //    outcomeVariable.RemoveNodes();
                //    outcomeVariable.Add(outcome.Value.ToString().ToValueElement().AddDefaultNamespace(Root.GetDefaultNamespace()));
                //}
            }
        }

        public void PersistTestResultOutcome(string testIdentifier, string outcomeIdentifier)
        {
            if (TestResults.ContainsKey(testIdentifier) && TestResults[testIdentifier].OutcomeVariables.ContainsKey(outcomeIdentifier))
            {
                var outcome = TestResults[testIdentifier].OutcomeVariables[outcomeIdentifier];
                var testResult = this
                    .FindElementsByElementAndAttributeValue("testResult", "identifier", testIdentifier)
                    .FirstOrDefault();
                var outcomeVariable = testResult?
                               .FindElementsByElementAndAttributeValue("outcomeVariable", "identifier", outcome.Identifier)
                               .FirstOrDefault();

                if (outcomeVariable != null)
                {
                    outcomeVariable.Descendants().FirstOrDefault()?.Remove();
                    outcomeVariable.Add(outcome.ToElement().AddDefaultNamespace(Root.GetDefaultNamespace())); // get rid of XCData 
                }
                else
                {
                    if (testResult == null)
                    {
                        AddTestResult(testIdentifier);
                    }
                    testResult.Add(outcome.ToElement().AddDefaultNamespace(Root.GetDefaultNamespace()));
                }
            }
            else
            {
                _logger.LogError($"{SourcedId}: - Cannot find testresult: {testIdentifier} outcome: {outcomeIdentifier} ");
            }
        }

        private T GetResult<T>(XElement resultElement) where T : IQtiResult, new()
        {
            return new T
            {
                Identifier = resultElement.Identifier(),
                OutcomeVariables = resultElement.FindElementsByName("outcomeVariable")
                               .Select(outcomeVariable =>
                               {
                                   return new OutcomeVariable
                                   {

                                       Identifier = outcomeVariable.Identifier(),
                                       BaseType = outcomeVariable.GetAttributeValue("baseType").ToBaseType(),
                                       Cardinality = outcomeVariable.GetAttributeValue("cardinality").ToCardinality(),
                                       Value = outcomeVariable.FindElementsByName("value").FirstOrDefault()?.Value?.RemoveXData()
                                   };
                               })?.ToDictionary(outcome => outcome.Identifier, outcome => outcome),
                ResponseVariables = resultElement.FindElementsByName("responseVariable").Select(responseVariable =>
                {
                    var values = responseVariable.FindElementsByName("value").Select(value =>
                    {
                        return value.Value?.RemoveXData();
                    }).ToList();
                    return new ResponseVariable
                    {
                        Identifier = responseVariable.Identifier(),
                        BaseType = responseVariable.GetAttributeValue("baseType").ToBaseType(),
                        Value = string.Join("&", values.ToArray()),
                        Values = values
                    };
                }).ToDictionary(r => r.Identifier, r => r)
            };
        }
    }
}
