using Microsoft.Extensions.Logging;
using Moq;
using Citolab.QTI.ScoringEngine.Model;
using Citolab.QTI.ScoringEngine.ResponseProcessing;
using System.IO;
using System.Xml.Linq;
using Xunit;

namespace Citolab.QTI.ScoringEngine.Tests.ResponseProcessingTests
{
    public class Gonneke14ChoiceInteractionTests
    {
        private const string ItemPath = "Resources/30/ResponseProcessing/Gonneke-16-04-2026-test-14.xml";
        private const string ItemId = "Gonneke-16-04-2026-test-14";
        private const string CorrectChoice = "SIMPLE_CHOICE_2fdfc1ee-4f98-46e6-bb13-7e768b885a3c";
        private const string IncorrectChoice = "SIMPLE_CHOICE_1c6812e2-c84f-4d63-91ba-e2dcba0bef7f";

        [Fact]
        public void Gonneke14_CorrectChoice_ScoresOne()
        {
            var logger = new Mock<ILogger>().Object;
            var assessmentItem = new AssessmentItem(logger, XDocument.Load(File.OpenRead(ItemPath)), TestHelper.GetExpressionFactory());
            var assessmentResult = TestHelper.GetBasicAssessmentResult();
            assessmentResult.AddCandidateResponse(assessmentItem.Identifier, "RESPONSE1", CorrectChoice, BaseType.Identifier, Cardinality.Single);

            ResponseProcessor.Process(assessmentItem, assessmentResult, logger);

            var score = assessmentResult.GetScoreForItem(ItemId, "SCORE");
            Assert.Equal("1", score);
        }

        [Fact]
        public void Gonneke14_IncorrectChoice_ScoresZero()
        {
            var logger = new Mock<ILogger>().Object;
            var assessmentItem = new AssessmentItem(logger, XDocument.Load(File.OpenRead(ItemPath)), TestHelper.GetExpressionFactory());
            var assessmentResult = TestHelper.GetBasicAssessmentResult();
            assessmentResult.AddCandidateResponse(assessmentItem.Identifier, "RESPONSE1", IncorrectChoice, BaseType.Identifier, Cardinality.Single);

            ResponseProcessor.Process(assessmentItem, assessmentResult, logger);

            var score = assessmentResult.GetScoreForItem(ItemId, "SCORE");
            Assert.Equal("0", score);
        }

        [Fact]
        public void Gonneke14_AllChoicesScoredCorrectly()
        {
            var logger = new Mock<ILogger>().Object;
            var assessmentItem = new AssessmentItem(logger, XDocument.Load(File.OpenRead(ItemPath)), TestHelper.GetExpressionFactory());
            var assessmentResult = TestHelper.GetBasicAssessmentResult();
            assessmentResult.AddCandidateResponse(assessmentItem.Identifier, "RESPONSE1", "SIMPLE_CHOICE_1c6812e2-c84f-4d63-91ba-e2dcba0bef7f", BaseType.Identifier, Cardinality.Single);
            ResponseProcessor.Process(assessmentItem, assessmentResult, logger);
            var deel1 = assessmentResult.GetScoreForItem(ItemId, "SCORE");

            assessmentResult.ChangeResponse(ItemId, "RESPONSE1", "SIMPLE_CHOICE_5d788595-b581-40c6-906d-dd90ddac38fd");
            ResponseProcessor.Process(assessmentItem, assessmentResult, logger);
            var deel2 = assessmentResult.GetScoreForItem(ItemId, "SCORE");

            assessmentResult.ChangeResponse(ItemId, "RESPONSE1", "SIMPLE_CHOICE_23dd4de1-3d43-40fb-8dc7-07b5df88938d");
            ResponseProcessor.Process(assessmentItem, assessmentResult, logger);
            var deel3 = assessmentResult.GetScoreForItem(ItemId, "SCORE");

            assessmentResult.ChangeResponse(ItemId, "RESPONSE1", CorrectChoice);
            ResponseProcessor.Process(assessmentItem, assessmentResult, logger);
            var deel4 = assessmentResult.GetScoreForItem(ItemId, "SCORE");

            assessmentResult.ChangeResponse(ItemId, "RESPONSE1", "SIMPLE_CHOICE_4b897710-f829-4af1-a246-46447840525d");
            ResponseProcessor.Process(assessmentItem, assessmentResult, logger);
            var deel5 = assessmentResult.GetScoreForItem(ItemId, "SCORE");

            Assert.Equal("0", deel1);
            Assert.Equal("0", deel2);
            Assert.Equal("0", deel3);
            Assert.Equal("1", deel4);
            Assert.Equal("0", deel5);
        }
    }
}
