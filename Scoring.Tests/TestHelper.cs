using Microsoft.Extensions.Logging;
using Moq;
using Citolab.QTI.ScoringEngine.Model;
using Citolab.QTI.ScoringEngine.ResponseProcessing;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;
using Citolab.QTI.ScoringEngine.Helper;
using Citolab.QTI.ScoringEngine.OutcomeProcessing;
using System.IO;

namespace Citolab.QTI.ScoringEngine.Tests
{
    public static class TestHelper
    {
        public static ResponseProcessorContext GetDefaultResponseProcessingContext(AssessmentItem assessmentItem)
        {
            var logger = new Mock<ILogger>().Object;
            var context = new ResponseProcessorContext(logger, GetBasicAssessmentResult(), assessmentItem);
            if (assessmentItem.OutcomeDeclarations != null)
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

        public static AssessmentResult GetBasicAssessmentResult()
        {
            var logger = new Mock<ILogger>().Object;
            var assessmentResultBase = XDocument.Parse("<assessmentResult xmlns:xsi=\"http://www.w3.org/2001/XMLSchema-instance\" xmlns:xsd=\"http://www.w3.org/2001/XMLSchema\" xmlns=\"http://www.imsglobal.org/xsd/imsqti_result_v2p2\"><context sourcedId=\"900001\" /></assessmentResult>");
            return new AssessmentResult(logger, assessmentResultBase);
        }
        public static AssessmentItem CreateAssessmentItem(List<OutcomeDeclaration> outcomes)
        {
            return CreateAssessmentItem("ITM-12345", outcomes);
        }

        public static AssessmentItem CreateAssessmentItem(string itemIdentifier, List<OutcomeDeclaration> outcomes)
        {
            var logger = new Mock<ILogger>().Object;
            var asssessmentItemDocument = XDocument.Parse($"<assessmentItem xmlns:xsi=\"http://www.w3.org/2001/XMLSchema-instance\" xmlns:xsd=\"http://www.w3.org/2001/XMLSchema\" xsi:schemaLocation=\"http://www.imsglobal.org/xsd/imsqti_v2p2 ../controlxsds/imsqti_v2p2p1.xsd\" title=\"test\" identifier=\"{itemIdentifier}\" label=\"32fyz6\" timeDependent=\"false\" xmlns=\"http://www.imsglobal.org/xsd/imsqti_v2p2\">\n<responseProcessing>\n</responseProcessing>\n</assessmentItem>");
            var assessmentItem = new AssessmentItem(logger, asssessmentItemDocument);
            assessmentItem.OutcomeDeclarations = outcomes.ToDictionary(o => o.Identifier, o => o);
            return assessmentItem;
        }

        public static AssessmentResult AddVariablesAndStartOutcomeProcessing(string assessmentTestPath, string assessmentResultPath, Mock<ILogger> mockLogger = null)
        {
            return StartOutcomeProcessing(assessmentTestPath, assessmentResultPath, mockLogger, true);
        }
        public static AssessmentResult StartOutcomeProcessing(string assessmentTestPath, string assessmentResultPath)
        {
            return StartOutcomeProcessing(assessmentTestPath, assessmentResultPath, null, false);
        }
        public static AssessmentResult StartOutcomeProcessing(string assessmentTestPath, string assessmentResultPath, Mock<ILogger> mockLogger, bool addVariables)
        {
            if (mockLogger == null)
            {
                mockLogger = new Mock<ILogger>();
            }
            var logger = mockLogger.Object;
            var assessmentTest = new AssessmentTest(logger, XDocument.Load(File.OpenRead($"Resources\\OutcomeProcessing\\{assessmentTestPath}.xml")));
            // add total and and category scores if they are not already in 
            // the assessmentTest.
            if (addVariables)
            {
                assessmentTest.AddTotalAndCategoryScores();
            }

            var assessmentResult = new AssessmentResult(logger, XDocument.Load(File.OpenRead($"Resources\\OutcomeProcessing\\{assessmentResultPath}.xml")));
            var outcomeProcessing = new OutcomeProcessor();

            return outcomeProcessing.Process(assessmentTest, assessmentResult, logger);
        }
    }
}
