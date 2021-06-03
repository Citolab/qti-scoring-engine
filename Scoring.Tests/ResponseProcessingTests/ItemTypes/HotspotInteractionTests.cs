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
using Citolab.QTI.ScoringEngine.Tests;

namespace ScoringEngine.Tests.ResponseProcessingTests.ItemTypes
{
    public class HotspotInteractionTests
    {
        [Fact]
        public void IMS_ExampleHotspotResponseProcessing_Correct()
        {
            var logger = new Mock<ILogger>().Object;

            var assessmentItem = new AssessmentItem(logger, XDocument.Load(File.OpenRead("Resources/2x/ResponseProcessing/IMS-examples/hotspot.xml")));
            var assessmentResult = TestHelper.GetBasicAssessmentResult();
            assessmentResult.AddCandidateResponse
                (assessmentItem.Identifier, "RESPONSE", "A"
                , BaseType.Identifier, Cardinality.Single);

            ResponseProcessor.Process(assessmentItem, assessmentResult, logger);

            var result = assessmentResult.GetScoreForItem(assessmentItem.Identifier, "SCORE");
            Assert.Equal("1", result);
        }
        [Fact]
        public void IMS_ExampleHotspotResponseProcessing_Inorrect()
        {
            var logger = new Mock<ILogger>().Object;

            var assessmentItem = new AssessmentItem(logger, XDocument.Load(File.OpenRead("Resources/2x/ResponseProcessing/IMS-examples/hotspot.xml")));
            var assessmentResult = TestHelper.GetBasicAssessmentResult();
            assessmentResult.AddCandidateResponse
                (assessmentItem.Identifier, "RESPONSE", "B"
                , BaseType.Identifier, Cardinality.Single);

            ResponseProcessor.Process(assessmentItem, assessmentResult, logger);

            var result = assessmentResult.GetScoreForItem(assessmentItem.Identifier, "SCORE");
            Assert.Equal("0", result);
        }
    }
}
