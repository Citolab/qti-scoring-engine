using Microsoft.Extensions.Logging;
using Moq;
using Citolab.QTI.Scoring.Model;
using Citolab.QTI.Scoring.ResponseProcessing;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;
using Citolab.QTI.Scoring.Helper;
using Citolab.QTI.Scoring.OutcomeProcessing;
using System.IO;
using Citolab.QTI.Scoring.ResponseProcessing.Interfaces;

namespace Citolab.QTI.Scoring.Tests
{
    public static class TestHelper
    {
        internal static ResponseProcessorContext GetDefaultResponseProcessingContext(AssessmentItem assessmentItem)
        {
            var logger = new Mock<ILogger>().Object;
            var context = new ResponseProcessorContext(logger, GetBasicAssessmentResult(), assessmentItem);
            if (assessmentItem?.OutcomeDeclarations != null)
            {
                if (context.ItemResult == null)
                {
                    context.ItemResult = new ItemResult()
                    {
                        Identifier = assessmentItem.Identifier,
                        OutcomeVariables = new Dictionary<string, OutcomeVariable>(),
                        ResponseVariables = new Dictionary<string, ResponseVariable>()
                    };
                }
                assessmentItem.OutcomeDeclarations.Select(outcomeDeclaration =>
                {
                    return outcomeDeclaration.Value.CreateVariable();
                })
                    .ToList()
                .ForEach(outcome =>
                {
                    context.ItemResult.OutcomeVariables.Add(outcome.Identifier, outcome);
                });
            }

            return context;
        }

        internal static OutcomeProcessorContext GetDefaultOutcomeProcessingContext(AssessmentTest assessmentTest)
        {
            var logger = new Mock<ILogger>().Object;
            var context = new OutcomeProcessorContext(GetBasicAssessmentResult(), assessmentTest, logger);
            if (assessmentTest?.OutcomeDeclarations != null)
            {
                if (context.AssessmentResult == null)
                {
                    context.TestResult = new TestResult()
                    {
                        Identifier = assessmentTest.Identifier,
                        OutcomeVariables = new Dictionary<string, OutcomeVariable>(),
                        ResponseVariables = new Dictionary<string, ResponseVariable>()
                    };
                }
                assessmentTest.OutcomeDeclarations.Select(outcomeDeclaration =>
                {
                    return outcomeDeclaration.Value.CreateVariable();
                })
                    .ToList()
                .ForEach(outcome =>
                {
                    context.TestResult.OutcomeVariables.Add(outcome.Identifier, outcome);
                });
            }

            return context;
        }

        internal static AssessmentResult GetBasicAssessmentResult()
        {
            var logger = new Mock<ILogger>().Object;
            var assessmentResultBase = XDocument.Parse("<assessmentResult xmlns:xsi=\"http://www.w3.org/2001/XMLSchema-instance\" xmlns:xsd=\"http://www.w3.org/2001/XMLSchema\" xmlns=\"http://www.imsglobal.org/xsd/imsqti_result_v2p2\"><context sourcedId=\"900001\" /></assessmentResult>");
            return new AssessmentResult(logger, assessmentResultBase);
        }


        internal static AssessmentItem CreateAssessmentItem(List<OutcomeDeclaration> outcomes, List<ResponseDeclaration> responseDeclarations = null)
        {
            return CreateAssessmentItem("ITM-12345", outcomes, responseDeclarations);
        }

        internal static AssessmentItem CreateAssessmentItem(string itemIdentifier, List<OutcomeDeclaration> outcomes, List<ResponseDeclaration> responseDeclarations)
        {
            var logger = new Mock<ILogger>().Object;
            var asssessmentItemDocument = XDocument.Parse($"<assessmentItem xmlns:xsi=\"http://www.w3.org/2001/XMLSchema-instance\" xmlns:xsd=\"http://www.w3.org/2001/XMLSchema\" xsi:schemaLocation=\"http://www.imsglobal.org/xsd/imsqti_v2p2 ../controlxsds/imsqti_v2p2p1.xsd\" title=\"test\" identifier=\"{itemIdentifier}\" label=\"32fyz6\" timeDependent=\"false\" xmlns=\"http://www.imsglobal.org/xsd/imsqti_v2p2\">\n<responseProcessing>\n</responseProcessing>\n</assessmentItem>");
            var assessmentItem = new AssessmentItem(logger, asssessmentItemDocument);
            assessmentItem.OutcomeDeclarations = outcomes.ToDictionary(o => o.Identifier, o => o);
            assessmentItem.ResponseDeclarations = responseDeclarations == null ? new Dictionary<string, ResponseDeclaration>() :
                responseDeclarations.ToDictionary(r => r.Identifier, r => r);
            return assessmentItem;
        }

        internal static AssessmentTest CreateAssessmentTest(string testIdentifier, List<OutcomeDeclaration> outcomes)
        {
            var logger = new Mock<ILogger>().Object;
            var asssessmentTestDocument = XDocument.Parse($"<assessmentTest identifier=\"{testIdentifier}\"><outcomeProcessing></outcomeProcessing></assessmentTest>");
            var assessmentTest = new AssessmentTest(logger, asssessmentTestDocument)
            {
                OutcomeDeclarations = outcomes.ToDictionary(o => o.Identifier, o => o)
            };
            return assessmentTest;
        }

        internal static AssessmentResult AddVariablesAndStartOutcomeProcessing(string assessmentTestPath, string assessmentResultPath, Mock<ILogger> mockLogger = null)
        {
            return StartOutcomeProcessing(assessmentTestPath, assessmentResultPath, mockLogger, true);
        }
        internal static AssessmentResult StartOutcomeProcessing(string assessmentTestPath, string assessmentResultPath)
        {
            return StartOutcomeProcessing(assessmentTestPath, assessmentResultPath, null, false);
        }
        internal static AssessmentResult StartOutcomeProcessing(string assessmentTestPath, string assessmentResultPath, Mock<ILogger> mockLogger, bool addVariables)
        {
            if (mockLogger == null)
            {
                mockLogger = new Mock<ILogger>();
            }
            var assessmentDoc = XDocument.Load(File.OpenRead($"Resources\\OutcomeProcessing\\{assessmentTestPath}.xml"));
            // add total and and category scores if they are not already in 
            // the assessmentTest.
            if (addVariables)
            {
                assessmentDoc.AddTotalAndCategoryScores();
            }
            var logger = mockLogger.Object;
            var assessmentTest = new AssessmentTest(logger, assessmentDoc);
            var assessmentResult = new AssessmentResult(logger, XDocument.Load(File.OpenRead($"Resources\\OutcomeProcessing\\{assessmentResultPath}.xml")));
            

            return OutcomeProcessor.Process(assessmentTest, assessmentResult, logger);
        }
    }
    internal class ReturnFalse : IExecuteReponseProcessing
    {
        public string Name => "returnFalse";
        public bool Execute(XElement qtiElement, ResponseProcessorContext context)
        {
            return false;
        }
    }

    internal class ReturnTrue : IExecuteReponseProcessing
    {
        public string Name => "returnTrue";
        public bool Execute(XElement qtiElement, ResponseProcessorContext context)
        {
            return true;
        }
    }

    internal class ReturnValue: ICalculateResponseProcessing
    {
        public string Name => "ReturnValue";
        public float Calculate(XElement qtiElement, ResponseProcessorContext context)
        {
            return float.Parse(qtiElement.GetAttributeValue("value"));
        }
    }

}



