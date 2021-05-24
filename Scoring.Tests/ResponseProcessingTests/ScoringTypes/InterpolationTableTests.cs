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

namespace Citolab.QTI.ScoringEngine.Tests.ResponseProcessingTests.ScoringTypes
{
    public class InterpolationTableTests
    {

        /// <summary>
        /// Verify that interpolation is applied and results in the correct raw_score and score
        /// </summary>
        /// <returns></returns>
        [Fact]
        public void ResponseProcessing_Interpolation_Correct_Score_And_Raw_Score()
        {
            var logger = new Mock<ILogger>().Object;

            var assessmentResult = new AssessmentResult(logger, XDocument.Load(File.OpenRead("Resources\\ResponseProcessing\\AssessmentResult_Interpolation.xml")));
            var assessmentItem = new AssessmentItem(logger, XDocument.Load(File.OpenRead("Resources\\ResponseProcessing\\083-Verklanking-speciale-tekens.xml")));

            var responseProcessing = new ResponseProcessor();
            responseProcessing.Process(assessmentItem, assessmentResult, logger);

            var scoreValue = assessmentResult.GetScoreForItem("ITM-083-Verklanking-speciale-tekens", "SCORE");
            var rawScoreValue = assessmentResult.GetScoreForItem("ITM-083-Verklanking-speciale-tekens", "RAW_SCORE");

            Assert.Equal("8", scoreValue);
            Assert.Equal("1", rawScoreValue);
        }
    }
}
