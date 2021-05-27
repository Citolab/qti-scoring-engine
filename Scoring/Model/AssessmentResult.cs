using Microsoft.Extensions.Logging;
using Citolab.QTI.Scoring.Const;
using Citolab.QTI.Scoring.Helper;
using Citolab.QTI.Scoring.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;

namespace Citolab.QTI.Scoring.Model
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

        public void PersistItemResultOutcome(string itemIdentifier, string outcomeIdentifier)
        {
            if (ItemResults.ContainsKey(itemIdentifier) && ItemResults[itemIdentifier].OutcomeVariables.ContainsKey(outcomeIdentifier))
            {
                var outcome = ItemResults[itemIdentifier].OutcomeVariables[outcomeIdentifier];
                var itemResult = this
                    .FindElementsByElementAndAttributeValue("itemResult", "identifier", itemIdentifier)
                    .FirstOrDefault();
                var outcomeVariable = itemResult?
                               .FindElementsByElementAndAttributeValue("outcomeVariable", "identifier", outcome.Identifier)
                               .FirstOrDefault();

                if (outcomeVariable != null)
                {
                    outcomeVariable.Descendants().FirstOrDefault()?.Remove();
                    outcomeVariable.Add(outcome.Value.ToString().ToValueElement()
                        .AddDefaultNamespace(Root.GetDefaultNamespace()));
                }
                else
                {
                    if (itemResult == null)
                    {
                        AddItemResult(itemIdentifier);
                    }
                    itemResult.Add(outcome.ToElement().AddDefaultNamespace(Root.GetDefaultNamespace()));
                }
            }
            else
            {
                _logger.LogError($"{SourcedId}: - Cannot find itemresult: {itemIdentifier} outcome: {outcomeIdentifier} ");
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
