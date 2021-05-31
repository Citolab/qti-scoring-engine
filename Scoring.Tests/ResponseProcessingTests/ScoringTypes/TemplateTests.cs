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
    public class TemplateTests
    {
        /// <summary>
        /// Verify that score 1 is equal to the first answer in the mapping
        /// </summary>
        [Fact]
        public void MapEntry_From_Template()
        {
            var logger = new Mock<ILogger>().Object;
            XDocument assessmentResultXDocument = null;
            XDocument assessmentItemXDocument = null;
            using (var assessmentResultStream = File.OpenRead("Resources/ResponseProcessing/AssessmentResult_Mapping_A.xml"))
            {
                using (var itemStream = File.OpenRead("Resources/ResponseProcessing/ITM-50069_Mapping.xml"))
                {
                    assessmentResultXDocument = XDocument.Load(assessmentResultStream);
                    assessmentItemXDocument = XDocument.Load(itemStream);
                };
            }
            var assessmentResult = new AssessmentResult(logger, assessmentResultXDocument);
            var assessmentItem = new AssessmentItem(logger, assessmentItemXDocument);

            

            ResponseProcessor.Process(assessmentItem, assessmentResult, logger);
            var scoreValueA = assessmentResult.GetScoreForItem("ITM-50069", "SCORE");

            // change the response
            assessmentResult.ChangeResponse("ITM-50069", "RESPONSE", "B");
            ResponseProcessor.Process(assessmentItem, assessmentResult, logger);
            var scoreValueB = assessmentResult.GetScoreForItem("ITM-50069", "SCORE");

            assessmentResult.ChangeResponse("ITM-50069", "RESPONSE", "C");
            ResponseProcessor.Process(assessmentItem, assessmentResult, logger);
            var scoreValueC = assessmentResult.GetScoreForItem("ITM-50069", "SCORE");

            Assert.Equal("0", scoreValueA);
            Assert.Equal("1", scoreValueB);
            Assert.Equal("2", scoreValueC);
        }
    }
}
