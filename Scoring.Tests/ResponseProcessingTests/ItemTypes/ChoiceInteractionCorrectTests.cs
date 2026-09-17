using Microsoft.Extensions.Logging;
using Moq;
using Citolab.QTI.ScoringEngine.Model;
using Citolab.QTI.ScoringEngine.ResponseProcessing;
using System.IO;
using System.Xml.Linq;
using Xunit;

namespace Citolab.QTI.ScoringEngine.Tests.ResponseProcessingTests
{
    public class ChoiceInteractionCorrectTests
    {
        private const string ItemPath = "Resources/30/ResponseProcessing/choice-interaction-correct.xml";
        private const string ItemId = "CHOICE-INTERACTION-CORRECT";
        private const string CorrectChoice = "CHOICE_4";
        private const string IncorrectChoice = "CHOICE_1";

        [Fact]
        public void CorrectChoice_ScoresOne()
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
        public void IncorrectChoice_ScoresZero()
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
        public void AllChoicesScoredCorrectly()
        {
            var logger = new Mock<ILogger>().Object;
            var assessmentItem = new AssessmentItem(logger, XDocument.Load(File.OpenRead(ItemPath)), TestHelper.GetExpressionFactory());
            var assessmentResult = TestHelper.GetBasicAssessmentResult();
            assessmentResult.AddCandidateResponse(assessmentItem.Identifier, "RESPONSE1", "CHOICE_1", BaseType.Identifier, Cardinality.Single);
            ResponseProcessor.Process(assessmentItem, assessmentResult, logger);
            var choice1 = assessmentResult.GetScoreForItem(ItemId, "SCORE");

            assessmentResult.ChangeResponse(ItemId, "RESPONSE1", "CHOICE_2");
            ResponseProcessor.Process(assessmentItem, assessmentResult, logger);
            var choice2 = assessmentResult.GetScoreForItem(ItemId, "SCORE");

            assessmentResult.ChangeResponse(ItemId, "RESPONSE1", "CHOICE_3");
            ResponseProcessor.Process(assessmentItem, assessmentResult, logger);
            var choice3 = assessmentResult.GetScoreForItem(ItemId, "SCORE");

            assessmentResult.ChangeResponse(ItemId, "RESPONSE1", CorrectChoice);
            ResponseProcessor.Process(assessmentItem, assessmentResult, logger);
            var choice4 = assessmentResult.GetScoreForItem(ItemId, "SCORE");

            assessmentResult.ChangeResponse(ItemId, "RESPONSE1", "CHOICE_5");
            ResponseProcessor.Process(assessmentItem, assessmentResult, logger);
            var choice5 = assessmentResult.GetScoreForItem(ItemId, "SCORE");

            Assert.Equal("0", choice1);
            Assert.Equal("0", choice2);
            Assert.Equal("0", choice3);
            Assert.Equal("1", choice4);
            Assert.Equal("0", choice5);
        }
    }
}
