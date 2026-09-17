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
    //<correctResponse>
    //  <value>118 184</value>
    //  <value>150 235</value>
    //  <value>96 114</value>
    //</correctResponse>
    //<areaMapping defaultValue="0">
    //  <areaMapEntry shape="circle" coords="118,184,12" mappedValue="1"/>
    //  <areaMapEntry shape="circle" coords="150,235,12" mappedValue="1"/>
    //  <areaMapEntry shape="circle" coords="96,114,12" mappedValue="1"/>
    //</areaMapping>
    //
    // The points below are written out rather than derived from the areaMapping, so that a
    // shape that parses its coords wrongly fails here instead of agreeing with itself.
    public class PositionObjectInteractionTests
    {
        private static string Score(List<string> responses)
        {
            var logger = new Mock<ILogger>().Object;

            var assessmentItem = new AssessmentItem(logger, XDocument.Load(File.OpenRead("Resources/2x/ResponseProcessing/IMS-examples/position_object.xml")), TestHelper.GetExpressionFactory());
            var assessmentResult = TestHelper.GetBasicAssessmentResult();

            assessmentResult.AddCandidateResponses
                (assessmentItem.Identifier, "RESPONSE", responses, BaseType.Point, Cardinality.Multiple);

            ResponseProcessor.Process(assessmentItem, assessmentResult, logger);

            return assessmentResult.GetScoreForItem(assessmentItem.Identifier, "SCORE");
        }

        [Fact]
        public void IMS_ExamplePositionObjectResponseProcessing_Correct()
        {
            Assert.Equal("3", Score(new List<string> { "118 184", "150 235", "96 114" }));
        }

        [Fact]
        public void IMS_ExamplePositionObjectResponseProcessing_OffCentreButInside_Correct()
        {
            // each point is 8 pixels off its centre, inside the radius of 12.
            Assert.Equal("3", Score(new List<string> { "126 184", "158 235", "104 114" }));
        }

        [Fact]
        public void IMS_ExamplePositionObjectProcessing_Partly_Correct()
        {
            Assert.Equal("2", Score(new List<string> { "118 184", "150 235" }));
        }

        [Fact]
        public void IMS_ExamplePositionObjectProcessing_OneJustOutsideItsRadius_Partly_Correct()
        {
            // 13 pixels right of its centre, one past the radius of 12.
            Assert.Equal("2", Score(new List<string> { "118 184", "150 235", "109 114" }));
        }

        [Fact]
        public void IMS_ExamplePositionObjectResponseProcessing_Incorrect()
        {
            Assert.Equal("0", Score(new List<string> { "0 0", "0 0", "0 0" }));
        }
    }
}
