using Microsoft.Extensions.Logging;
using Moq;
using Citolab.QTI.ScoringEngine.Model;
using Citolab.QTI.ScoringEngine.ResponseProcessing;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;
using Xunit;

namespace Citolab.QTI.ScoringEngine.Tests.ResponseProcessingTests
{
    public class ChoiceInterationTests
    {
        [Fact]
        public void ChoiceInteractionResponseProcessingDictotoom()
        {
            var logger = new Mock<ILogger>().Object;

            var assessmentResult = new AssessmentResult(logger, XDocument.Load(File.OpenRead("Resources/2x/ResponseProcessing/AssessmentResult_Add_OutcomeVariable.xml")));
            var assessmentItem = new AssessmentItem(logger, XDocument.Load(File.OpenRead("Resources/2x/ResponseProcessing/ITM-50069.xml")), TestHelper.GetExpressionFactory());


            ResponseProcessor.Process(assessmentItem, assessmentResult, logger);

            var scoreValue = assessmentResult.GetScoreForItem("ITM-50069", "SCORE");
            Assert.Equal("0", scoreValue);
        }

        /// <summary>
        /// Verify that the score is correct when there are multiple correct answers
        /// </summary>
        [Fact]
        public void ChoiceInteraction_Multiple_Keys()
        {
            var logger = new Mock<ILogger>().Object;

            var assessmentResult = new AssessmentResult(logger, XDocument.Load(File.OpenRead("Resources/2x/ResponseProcessing/AssessmentResult_Multiple_Responses_01.xml")));
            var assessmentItem = new AssessmentItem(logger, XDocument.Load(File.OpenRead("Resources/2x/ResponseProcessing/ITM-50066_Multiple_Keys.xml")), TestHelper.GetExpressionFactory());

            // A = correct

            ResponseProcessor.Process(assessmentItem, assessmentResult, logger);
            var scoreA = assessmentResult.GetScoreForItem("ITM-50066", "SCORE");
            // B = incorrect
            assessmentResult.ChangeResponse("ITM-50066", "RESPONSE", "B");
            ResponseProcessor.Process(assessmentItem, assessmentResult, logger);
            var scoreB = assessmentResult.GetScoreForItem("ITM-50066", "SCORE");

            // C = correct
            assessmentResult.ChangeResponse("ITM-50066", "RESPONSE", "C");
            ResponseProcessor.Process(assessmentItem, assessmentResult, logger);
            var scoreC = assessmentResult.GetScoreForItem("ITM-50066", "SCORE");

            Assert.Equal("1", scoreA);
            Assert.Equal("0", scoreB);
            Assert.Equal("1", scoreC);
        }

        [Fact]
        public void IMS_Example_ChoiceInteractionResponseProcessingDictotoom()
        {
            var logger = new Mock<ILogger>().Object;

            var assessmentItem = new AssessmentItem(logger, XDocument.Load(File.OpenRead("Resources/2x/ResponseProcessing/IMS-examples/choice.xml")), TestHelper.GetExpressionFactory());
            var assessmentResult = TestHelper.GetBasicAssessmentResult();
            assessmentResult.AddCandidateResponse(assessmentItem.Identifier, "RESPONSE", "ChoiceA", BaseType.Identifier, Cardinality.Single);

            ResponseProcessor.Process(assessmentItem, assessmentResult, logger);

            var scoreValueA = assessmentResult.GetScoreForItem(assessmentItem.Identifier, "SCORE");

            assessmentResult.ChangeResponse(assessmentItem.Identifier, "RESPONSE", "ChoiceB");
            ResponseProcessor.Process(assessmentItem, assessmentResult, logger);
            var scoreValueB = assessmentResult.GetScoreForItem(assessmentItem.Identifier, "SCORE");

            assessmentResult.ChangeResponse(assessmentItem.Identifier, "RESPONSE", "ChoiceC");
            ResponseProcessor.Process(assessmentItem, assessmentResult, logger);
            var scoreValueC = assessmentResult.GetScoreForItem(assessmentItem.Identifier, "SCORE");

            Assert.Equal("1", scoreValueA);
            Assert.Equal("0", scoreValueB);
            Assert.Equal("0", scoreValueC);
        }

    }
}
