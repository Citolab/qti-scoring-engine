using Citolab.QTI.ScoringEngine.Model;
using Citolab.QTI.ScoringEngine.ResponseProcessing;
using Citolab.QTI.ScoringEngine.Tests;
using Microsoft.Extensions.Logging;
using Moq;
using System.IO;
using System.Xml.Linq;
using Xunit;
namespace ScoringEngine.Tests.ResponseProcessingTests.ItemTypes
{
    // <correctResponse>
    //   <value>102 113</value>
    // </correctResponse>
    // <areaMapping defaultValue="0">
    //   <areaMapEntry shape="circle" coords="102,113,16" mappedValue="1"/>
    // </areaMapping>
    //
    // The points below are written out rather than derived from the areaMapping, so that a
    // shape that parses its coords wrongly fails here instead of agreeing with itself.
    public class SelectPointTests
    {
        private static string Score(string response)
        {
            var logger = new Mock<ILogger>().Object;

            var assessmentItem = new AssessmentItem(logger, XDocument.Load(File.OpenRead("Resources/2x/ResponseProcessing/IMS-examples/select_point.xml")), TestHelper.GetExpressionFactory());
            var assessmentResult = TestHelper.GetBasicAssessmentResult();

            assessmentResult.AddCandidateResponse
                (assessmentItem.Identifier, "RESPONSE", response, BaseType.Point, Cardinality.Single);

            ResponseProcessor.Process(assessmentItem, assessmentResult, logger);

            return assessmentResult.GetScoreForItem(assessmentItem.Identifier, "SCORE");
        }

        [Fact]
        public void IMS_ExampleSelectPointResponseProcessing_Correct()
        {
            Assert.Equal("1", Score("102 113"));
        }

        [Fact]
        public void IMS_ExampleSelectPointResponseProcessing_NearTheCentre_Correct()
        {
            // 8 pixels right of the centre, comfortably inside the radius of 16.
            Assert.Equal("1", Score("110 113"));
        }

        [Fact]
        public void IMS_ExampleSelectPointResponseProcessing_OnTheEdge_Correct()
        {
            // exactly on the circle's edge: a click on the hotspot boundary counts.
            Assert.Equal("1", Score("118 113"));
        }

        [Fact]
        public void IMS_ExampleSelectPointResponseProcessing_JustOutsideTheRadius_InCorrect()
        {
            // one pixel further out than the previous case.
            Assert.Equal("0", Score("119 113"));
        }

        [Fact]
        public void IMS_ExampleSelectPointResponseProcessing_InCorrect()
        {
            Assert.Equal("0", Score("0 0"));
        }
    }
}
