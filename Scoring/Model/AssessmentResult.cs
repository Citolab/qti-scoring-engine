using System.Collections.Generic;
using System.Linq;
using System.Xml.Linq;
using Citolab.QTI.ScoringEngine.Helpers;
using Citolab.QTI.ScoringEngine.Interfaces;
using Citolab.QTI.ScoringEngine.ResponseProcessing;
using Microsoft.Extensions.Logging;

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
            var itemResultElement = itemResult.ToElement().AddDefaultNamespace(Root.GetDefaultNamespace());
            itemResult.SourceElement = itemResultElement;
            ItemResults.Add(itemIdentifier, itemResult);
            Root.Add(itemResultElement);
        }

        public void AddTestResult(string testIdentifier)
        {
            var testResult = new TestResult
            {
                Identifier = testIdentifier,
                OutcomeVariables = new Dictionary<string, OutcomeVariable>(),
                ResponseVariables = new Dictionary<string, ResponseVariable>()
            };
            var testResultElement = testResult.ToElement().AddDefaultNamespace(Root.GetDefaultNamespace());
            testResult.SourceElement = testResultElement;
            TestResults.Add(testIdentifier, testResult);
            Root.Add(testResultElement);
        }

        /// <summary>
        /// The element an itemResult was read from. Results built by a caller rather than by
        /// InitItemResults have none yet, so it is looked up once and then kept - the point is
        /// to stop rescanning every descendant of the document for each outcome written.
        /// </summary>
        private XElement GetItemResultElement(string itemIdentifier)
        {
            var itemResult = ItemResults[itemIdentifier];
            if (itemResult.SourceElement == null)
            {
                itemResult.SourceElement = this
                    .FindElementsByElementAndAttributeValue("itemResult", "identifier", itemIdentifier)
                    .FirstOrDefault();
            }
            return itemResult.SourceElement;
        }

        private XElement GetTestResultElement(string testIdentifier)
        {
            var testResult = TestResults[testIdentifier];
            if (testResult.SourceElement == null)
            {
                testResult.SourceElement = this
                    .FindElementsByElementAndAttributeValue("testResult", "identifier", testIdentifier)
                    .FirstOrDefault();
            }
            return testResult.SourceElement;
        }

        public void PersistItemResultOutcome(string itemIdentifier, string outcomeIdentifier, ResponseProcessorContext context)
        {
            // add items result if there is no result yet.
            if (!ItemResults.ContainsKey(itemIdentifier))
            {
                AddItemResult(itemIdentifier);
            }
            var itemResultElement = GetItemResultElement(itemIdentifier);
            // if there is no outcome for this variable then add one.
            var outcomeExists = ItemResults[itemIdentifier].OutcomeVariables.ContainsKey(outcomeIdentifier);
            if (!outcomeExists)
            {
                var outcomeDeclaration = context.OutcomeDeclarations[outcomeIdentifier];
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
                    itemResultElement.Add(outcome.ToElement().AddDefaultNamespace(Root.GetDefaultNamespace()));
                }
                else if (outcomeVariable.GetAttributeValue("external-scored") == "human")
                {
                    // leave human scored outcomes alone
                    // do nothing
                }
                else
                {
                    outcomeVariable.ReplaceWith(outcome.ToElement().AddDefaultNamespace(Root.GetDefaultNamespace()));
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
                var testResultElement = GetTestResultElement(testIdentifier);
                if (testResultElement == null)
                {
                    _logger.LogError($"{SourcedId}: - testResult: {testIdentifier} has no element to write outcome: {outcomeIdentifier} to.");
                    return;
                }
                var outcomeVariable = testResultElement
                               .FindElementsByElementAndAttributeValue("outcomeVariable", "identifier", outcome.Identifier)
                               .FirstOrDefault();

                if (outcomeVariable != null)
                {
                    outcomeVariable.ReplaceWith(outcome.ToElement().AddDefaultNamespace(Root.GetDefaultNamespace()));
                }
                else
                {
                    testResultElement.Add(outcome.ToElement().AddDefaultNamespace(Root.GetDefaultNamespace()));
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
                SourceElement = resultElement,
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