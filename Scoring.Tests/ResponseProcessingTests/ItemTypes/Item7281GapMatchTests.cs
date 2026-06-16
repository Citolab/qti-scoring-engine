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
    public class Item7281GapMatchTests
    {
        private const string ItemPath = "Resources/30/ResponseProcessing/7281.xml";

        [Fact]
        public void Item7281_CorrectResponse_AG7_ScoresOne()
        {
            var logger = new Mock<ILogger>().Object;
            var assessmentItem = new AssessmentItem(logger, XDocument.Load(File.OpenRead(ItemPath)), TestHelper.GetExpressionFactory());
            var assessmentResult = TestHelper.GetBasicAssessmentResult();
            assessmentResult.AddCandidateResponses(assessmentItem.Identifier, "RESPONSE",
                new List<string> { "A G7" }, BaseType.DirectedPair, Cardinality.Multiple);

            ResponseProcessor.Process(assessmentItem, assessmentResult, logger);

            var score = assessmentResult.GetScoreForItem(assessmentItem.Identifier, "SCORE");
            Assert.Equal("1", score);
        }

        [Fact]
        public void Item7281_IncorrectResponse_ScoresZero()
        {
            var logger = new Mock<ILogger>().Object;
            var assessmentItem = new AssessmentItem(logger, XDocument.Load(File.OpenRead(ItemPath)), TestHelper.GetExpressionFactory());
            var assessmentResult = TestHelper.GetBasicAssessmentResult();
            assessmentResult.AddCandidateResponses(assessmentItem.Identifier, "RESPONSE",
                new List<string> { "A G6" }, BaseType.DirectedPair, Cardinality.Multiple);

            ResponseProcessor.Process(assessmentItem, assessmentResult, logger);

            var score = assessmentResult.GetScoreForItem(assessmentItem.Identifier, "SCORE");
            Assert.Equal("0", score);
        }
    }
}
