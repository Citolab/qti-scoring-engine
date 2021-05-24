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
            XDocument assessmentResultXDocument = null;
            XDocument assessmentItemXDocument = null;
            using (var assessmentResultStream = File.OpenRead("Resources\\ResponseProcessing\\AssessmentResult_Mapping_A.xml"))
            {
                using (var itemStream = File.OpenRead("Resources\\ResponseProcessing\\ITM-50069_Mapping.xml"))
                {
                    assessmentResultXDocument = XDocument.Load(assessmentResultStream);
                    assessmentItemXDocument = XDocument.Load(itemStream);
                };
            }
            var assessmentResult = new AssessmentResult(logger, assessmentResultXDocument);
            var assessmentItem = new AssessmentItem(logger, assessmentItemXDocument);
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
