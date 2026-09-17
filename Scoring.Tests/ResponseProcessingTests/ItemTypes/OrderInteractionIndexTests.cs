using Microsoft.Extensions.Logging;
using Moq;
using Citolab.QTI.ScoringEngine.Model;
using Citolab.QTI.ScoringEngine.ResponseProcessing;
using System.Collections.Generic;
using System.IO;
using System.Xml.Linq;
using Xunit;
using Citolab.QTI.ScoringEngine.Tests;

namespace ScoringEngine.Tests.ResponseProcessingTests.ItemTypes
{
    public class OrderInteractionIndexTests
    {
        private const string ItemPath = "Resources/30/ResponseProcessing/order-interaction-index.xml";

        private static string Score(List<string> response)
        {
            var logger = new Mock<ILogger>().Object;
            var assessmentItem = new AssessmentItem(logger, XDocument.Load(File.OpenRead(ItemPath)), TestHelper.GetExpressionFactory());
            var assessmentResult = TestHelper.GetBasicAssessmentResult();
            assessmentResult.AddCandidateResponses(assessmentItem.Identifier, "RESPONSE",
                response, BaseType.Identifier, Cardinality.Ordered);

            ResponseProcessor.Process(assessmentItem, assessmentResult, logger);

            return assessmentResult.GetScoreForItem(assessmentItem.Identifier, "SCORE");
        }

        [Fact]
        public void AllFivePositionsCorrect_ScoresTwo()
        {
            Assert.Equal("2", Score(new List<string> { "D", "A", "B", "C", "E" }));
        }

        [Fact]
        public void ThreePositionsCorrect_ScoresOne()
        {
            // D, A, B correct; C and E swapped.
            Assert.Equal("1", Score(new List<string> { "D", "A", "B", "E", "C" }));
        }

        [Fact]
        public void TwoPositionsCorrect_ScoresZero()
        {
            // only D (pos 1) and B (pos 3) are in the right place
            Assert.Equal("0", Score(new List<string> { "D", "C", "B", "E", "A" }));
        }
    }
}
