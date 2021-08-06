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
using Citolab.QTI.ScoringEngine.Helpers;
using Citolab.QTI.ScoringEngine.OutcomeProcessing;
using System.IO;
using Citolab.QTI.ScoringEngine.Interfaces;
using Citolab.QTI.ScoringEngine.Expressions.ConditionExpressions;
using Citolab.QTI.ScoringEngine.Const;

namespace Citolab.QTI.ScoringEngine.Tests
{
    public static class TestHelper
    {
        private static IExpressionFactory _expressionFactory;

        internal static IExpressionFactory GetExpressionFactory()
        {
            if (_expressionFactory == null)
            {
                var mockLog = new Mock<ILogger>();
                _expressionFactory = new ExpressionFactory(null, mockLog.Object);
            }
            return _expressionFactory;
        }
        internal static ResponseProcessorContext GetDefaultResponseProcessingContext(AssessmentItem assessmentItem)
        {
            return GetDefaultResponseProcessingContextAndLogger(assessmentItem).Context;
        }
        internal static (ResponseProcessorContext Context, Mock<ILogger> MockLog) GetDefaultResponseProcessingContextAndLogger(AssessmentItem assessmentItem)
        {
            var mockLog = new Mock<ILogger>();
            var logger = mockLog.Object;
            var assessmentResult = GetBasicAssessmentResult();
            if (assessmentItem != null)
            {
                var itemResult = new ItemResult()
                {
                    Identifier = assessmentItem.Identifier,
                    OutcomeVariables = new Dictionary<string, OutcomeVariable>(),
                    ResponseVariables = new Dictionary<string, ResponseVariable>()
                };
                assessmentItem.OutcomeDeclarations.Values.ToList().ForEach(outcomeDeclaration =>
                {
                    itemResult.OutcomeVariables.Add(outcomeDeclaration.Identifier, outcomeDeclaration.ToVariable());
                });
                assessmentResult.ItemResults.Add(itemResult.Identifier, itemResult);
            }
            var context = new ResponseProcessorContext(logger, assessmentResult, assessmentItem);
            return (context, mockLog);
        }

        internal static XDocument GetDocument(string file)
        {
            XDocument xDoc = null;
            using (var openRead = File.OpenRead(file))
            {
                xDoc = XDocument.Load(openRead);
            };
            return xDoc;
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

            var expressionFactory = new ExpressionFactory(null, logger);
            var assessmentItem = new AssessmentItem(logger, asssessmentItemDocument, expressionFactory)
            {
                OutcomeDeclarations = outcomes.ToDictionary(o => o.Identifier, o => o),
                ResponseDeclarations = responseDeclarations == null ? new Dictionary<string, ResponseDeclaration>() :
                responseDeclarations.ToDictionary(r => r.Identifier, r => r)
            };
            return assessmentItem;
        }

        internal static AssessmentTest CreateAssessmentTest(string testIdentifier, List<OutcomeDeclaration> outcomes)
        {
            var logger = new Mock<ILogger>().Object;
            var asssessmentTestDocument = XDocument.Parse($"<assessmentTest identifier=\"{testIdentifier}\"><outcomeProcessing></outcomeProcessing></assessmentTest>");
            var expressionFactory = new ExpressionFactory(null, logger);
            var assessmentTest = new AssessmentTest(logger, asssessmentTestDocument, expressionFactory)
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
            var assessmentDoc = XDocument.Load(File.OpenRead($"Resources/2x/OutcomeProcessing/{assessmentTestPath}.xml"));
            // add total and and category scores if they are not already in 
            // the assessmentTest.
            if (addVariables)
            {
                assessmentDoc.AddTotalAndCategoryScores();
            }
            var logger = mockLogger.Object;
            var expressionFactory = new ExpressionFactory(null, logger);
            var assessmentTest = new AssessmentTest(logger, assessmentDoc, expressionFactory);
            var assessmentResult = new AssessmentResult(logger, XDocument.Load(File.OpenRead($"Resources/2x/OutcomeProcessing/{assessmentResultPath}.xml")));


            return OutcomeProcessor.Process(assessmentTest, assessmentResult, logger);
        }
    }
    internal class ReturnFalse : ConditionExpressionBase
    {
        public static void Init()
        {
            if (!Mappings.ConditionalExpressions.ContainsKey("returnFalse"))
            {
                Mappings.ConditionalExpressions.Add("returnFalse", typeof(ReturnFalse));
            }
        }
        public override bool Execute(IProcessingContext context)
        {
            return false;
        }
    }

    internal class ReturnTrue : ConditionExpressionBase
    {
        public static void Init()
        {
            if (!Mappings.ConditionalExpressions.ContainsKey("returnTrue"))
            {
                Mappings.ConditionalExpressions.Add("returnTrue", typeof(ReturnTrue));
            }
        }
        public override bool Execute(IProcessingContext context)
        {
            return true;
        }
    }

}



