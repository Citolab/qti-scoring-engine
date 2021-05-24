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

namespace Scoring.Tests.ResponseProcessingTests.ScoringTypes
{
    public class MapEntryTests
    {

        /// <summary>
        /// Verify that score 1 is equal to the first answer in the mapping
        /// </summary>
        [Fact]
        public void ChoiceInteraction_MapEntry_From_Template()
        {
            var logger = new Mock<ILogger>().Object;
            AssessmentResult assessmentResult = null;
            AssessmentItem assessmentItem = null;
            using (var assessmentStream = File.OpenRead("Resources\\ResponseProcessing\\AssessmentResult_Mapping_A.xml"))
            {
                using (var itemStream = File.OpenRead("Resources\\ResponseProcessing\\ITM-50069_Mapping.xml"))
                {
                    assessmentResult = new AssessmentResult(logger, XDocument.Load(assessmentStream));
                    assessmentItem = new AssessmentItem(logger, XDocument.Load(itemStream));
                };
            }

            var responseProcessing = new ResponseProcessing();

            responseProcessing.Process(assessmentItem, assessmentResult, logger);
            var scoreValueA = assessmentResult.GetScoreForItem("ITM-50069", "SCORE");

            // change the response
            assessmentResult.ChangeResponse("ITM-50069", "RESPONSE", "B");
            responseProcessing.Process(assessmentItem, assessmentResult, logger);
            var scoreValueB = assessmentResult.GetScoreForItem("ITM-50069", "SCORE");

            assessmentResult.ChangeResponse("ITM-50069", "RESPONSE", "C");
            responseProcessing.Process(assessmentItem, assessmentResult, logger);
            var scoreValueC = assessmentResult.GetScoreForItem("ITM-50069", "SCORE");

            Assert.Equal("0", scoreValueA);
            Assert.Equal("1", scoreValueB);
            Assert.Equal("2", scoreValueC);
        }
    }
}
