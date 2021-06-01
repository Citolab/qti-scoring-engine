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
    //<correctResponse>
	//	<value>A</value>
	//	<value>D</value>
	//	<value>C</value>
	//	<value>B</value>
	//</correctResponse>
    public class GraphicAssociateInteractionTests
    {
        [Fact]
        public void IMS_ExampleGraphicOrderInteractionResponseProcessing_Correct()
        {
            var logger = new Mock<ILogger>().Object;

            var assessmentItem = new AssessmentItem(logger, XDocument.Load(File.OpenRead("Resources/ResponseProcessing/IMS-examples/graphic_associate.xml")));
            var assessmentResult = TestHelper.GetBasicAssessmentResult();
            assessmentResult.AddCandidateResponses
                (assessmentItem.Identifier, "RESPONSE", new List<string>{
                    "C B", "C D"
                }
                , BaseType.Identifier, Cardinality.Ordered);

            ResponseProcessor.Process(assessmentItem, assessmentResult, logger);

            var result = assessmentResult.GetScoreForItem(assessmentItem.Identifier, "SCORE");
            Assert.Equal("2", result);
        }

        [Fact]
        public void IMS_ExampleGraphicOrderInteractionResponseProcessing_Correct2()
        {
            var logger = new Mock<ILogger>().Object;

            var assessmentItem = new AssessmentItem(logger, XDocument.Load(File.OpenRead("Resources/ResponseProcessing/IMS-examples/graphic_associate.xml")));
            var assessmentResult = TestHelper.GetBasicAssessmentResult();
            assessmentResult.AddCandidateResponses
                (assessmentItem.Identifier, "RESPONSE", new List<string>{
                    "D C", "B C"
                }
                , BaseType.Identifier, Cardinality.Ordered);

            ResponseProcessor.Process(assessmentItem, assessmentResult, logger);

            var result = assessmentResult.GetScoreForItem(assessmentItem.Identifier, "SCORE");
            Assert.Equal("2", result);
        }

        [Fact]
        public void IMS_ExampleGraphicOrderInteractionResponseProcessing_Partly_Correct()
        {
            var logger = new Mock<ILogger>().Object;

            var assessmentItem = new AssessmentItem(logger, XDocument.Load(File.OpenRead("Resources/ResponseProcessing/IMS-examples/graphic_associate.xml")));
            var assessmentResult = TestHelper.GetBasicAssessmentResult();
            assessmentResult.AddCandidateResponses
                (assessmentItem.Identifier, "RESPONSE", new List<string>{
                    "D C"
                }
                , BaseType.Identifier, Cardinality.Ordered);

            ResponseProcessor.Process(assessmentItem, assessmentResult, logger);

            var result = assessmentResult.GetScoreForItem(assessmentItem.Identifier, "SCORE");
            Assert.Equal("1", result);
        }

        [Fact]
        public void IMS_ExampleGraphicOrderInteractionResponseProcessing_Incorrect()
        {
            var logger = new Mock<ILogger>().Object;

            var assessmentItem = new AssessmentItem(logger, XDocument.Load(File.OpenRead("Resources/ResponseProcessing/IMS-examples/graphic_associate.xml")));
            var assessmentResult = TestHelper.GetBasicAssessmentResult();
            assessmentResult.AddCandidateResponses
                (assessmentItem.Identifier, "RESPONSE", new List<string>{
                    "D B", "B A"
                }
                , BaseType.Identifier, Cardinality.Ordered);

            ResponseProcessor.Process(assessmentItem, assessmentResult, logger);

            var result = assessmentResult.GetScoreForItem(assessmentItem.Identifier, "SCORE");
            Assert.Equal("0", result);
        }
    }
}
