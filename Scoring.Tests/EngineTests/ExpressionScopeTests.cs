using Citolab.QTI.ScoringEngine;
using Citolab.QTI.ScoringEngine.Helpers;
using Citolab.QTI.ScoringEngine.Model;
using Citolab.QTI.ScoringEngine.Tests;
using Microsoft.Extensions.Logging;
using Moq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Xml.Linq;
using Xunit;

namespace ScoringEngine.Tests.EngineTests
{
    /// <summary>
    /// Expressions declare which processing they cannot be used in. That declaration is now
    /// enforced when the expression tree is built, rather than being ignored until something
    /// fails deeper in.
    /// </summary>
    public class ExpressionScopeTests
    {
        private const string ItemHeader = @"<assessmentItem xmlns=""http://www.imsglobal.org/xsd/imsqti_v2p2"" identifier=""ITM-SCOPE"" title=""t"" timeDependent=""false"">
  <responseDeclaration identifier=""RESPONSE"" cardinality=""single"" baseType=""string"">
    <correctResponse><value>test</value></correctResponse>
  </responseDeclaration>
  <outcomeDeclaration identifier=""SCORE"" cardinality=""single"" baseType=""float""><defaultValue><value>0</value></defaultValue></outcomeDeclaration>
  <responseProcessing>";

        private static XDocument ItemWithResponseProcessing(string rules) =>
            XDocument.Parse($"{ItemHeader}{rules}</responseProcessing></assessmentItem>");

        private static (string Score, Mock<ILogger> Log) Score(XDocument item)
        {
            var log = new Mock<ILogger>();
            var result = TestHelper.GetBasicAssessmentResult();
            result.AddCandidateResponse("ITM-SCOPE", "RESPONSE", "test", BaseType.String, Cardinality.Single);
            var scored = new Citolab.QTI.ScoringEngine.ScoringEngine().ProcessResponses(new ResponseProcessingContext
            {
                AssessmentItems = new List<XDocument> { item },
                AssessmentmentResults = new List<XDocument> { result },
                Logger = log.Object
            });
            return (scored[0].GetScoreForItem("ITM-SCOPE", "SCORE"), log);
        }

        private static void AssertLoggedError(Mock<ILogger> log) =>
            log.Verify(l => l.Log(LogLevel.Error, It.IsAny<EventId>(), It.IsAny<It.IsAnyType>(),
                It.IsAny<Exception>(), It.IsAny<Func<It.IsAnyType, Exception, string>>()), Times.AtLeastOnce);

        [Fact]
        public void ATestOnlyExpressionInsideResponseProcessingIsReportedNotThrown()
        {
            // qti-test-variables only works against an OutcomeProcessorContext; used here it
            // used to reach an unguarded cast and throw InvalidCastException.
            var item = ItemWithResponseProcessing(@"
    <responseCondition><responseIf>
      <match><variable identifier=""RESPONSE""/><correct identifier=""RESPONSE""/></match>
      <setOutcomeValue identifier=""SCORE""><testVariables variableIdentifier=""SCORE""/></setOutcomeValue>
    </responseIf></responseCondition>");

            var (score, log) = Score(item);

            Assert.Equal("0", score);
            AssertLoggedError(log);
        }

        [Fact]
        public void AnOutcomeOnlyRuleInsideResponseProcessingIsSkipped()
        {
            var item = ItemWithResponseProcessing(@"
    <outcomeCondition><outcomeIf>
      <match><variable identifier=""RESPONSE""/><correct identifier=""RESPONSE""/></match>
      <setOutcomeValue identifier=""SCORE""><baseValue baseType=""float"">1</baseValue></setOutcomeValue>
    </outcomeIf></outcomeCondition>");

            var (score, log) = Score(item);

            Assert.Equal("0", score);
            AssertLoggedError(log);
        }

        [Fact]
        public void AnUnknownRuleIsSkippedRatherThanLeftInTheTreeAsNull()
        {
            // an unrecognised rule produced a null entry in the expression list, which then
            // threw NullReferenceException when the list was executed.
            var item = ItemWithResponseProcessing(@"
    <someRuleThatDoesNotExist/>
    <responseCondition><responseIf>
      <match><variable identifier=""RESPONSE""/><correct identifier=""RESPONSE""/></match>
      <setOutcomeValue identifier=""SCORE""><baseValue baseType=""float"">1</baseValue></setOutcomeValue>
    </responseIf></responseCondition>");

            var (score, log) = Score(item);

            // the rules that are understood still run
            Assert.Equal("1", score);
            AssertLoggedError(log);
        }

        [Fact]
        public void TestVariablesStillWorksWhereItBelongs()
        {
            // the guard must not fire for the outcome processing this expression is meant for
            var assessmentResult = TestHelper.StartOutcomeProcessing("Test_Toets_TestVariables", "AssessmentResult_Correct");
            Assert.Equal("2", assessmentResult.GetScoreForTest("TST-Test_toets", "SCORE"));
        }
    }
}
