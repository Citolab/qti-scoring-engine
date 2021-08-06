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
    //  <value>GLA A</value>
    //  <value>EDI B</value>
    //  <value>MAN C</value>
    //</correctResponse>
    //<mapping lowerBound="0" defaultValue="-1">
    //  <mapEntry mapKey="GLA A" mappedValue="1"/>
    //  <mapEntry mapKey="EDI B" mappedValue="1"/>
    //  <mapEntry mapKey="MAN C" mappedValue="1"/>
    //</mapping>
    public class GraphicGapMatchInteractionTests
    {
        [Fact]
        public void IMS_ExampleGraphicGapMatchInteractionResponseProcessing_Correct()
        {
            var logger = new Mock<ILogger>().Object;

            var assessmentItem = new AssessmentItem(logger, XDocument.Load(File.OpenRead("Resources/2x/ResponseProcessing/IMS-examples/graphic_gap_match.xml")), TestHelper.GetExpressionFactory());
            var assessmentResult = TestHelper.GetBasicAssessmentResult();
            assessmentResult.AddCandidateResponses
                (assessmentItem.Identifier, "RESPONSE", new List<string>{
                    "GLA A", "EDI B", "MAN C"
                }
                , BaseType.DirectedPair, Cardinality.Multiple);

            ResponseProcessor.Process(assessmentItem, assessmentResult, logger);

            var result = assessmentResult.GetScoreForItem(assessmentItem.Identifier, "SCORE");
            Assert.Equal("3", result);
        }

        [Fact]
        public void IMS_ExampleGraphicGapMatchInteractionResponseProcessing_Correct2()
        {
            var logger = new Mock<ILogger>().Object;

            var assessmentItem = new AssessmentItem(logger, XDocument.Load(File.OpenRead("Resources/2x/ResponseProcessing/IMS-examples/graphic_gap_match.xml")), TestHelper.GetExpressionFactory());
            var assessmentResult = TestHelper.GetBasicAssessmentResult();
            assessmentResult.AddCandidateResponses
                (assessmentItem.Identifier, "RESPONSE", new List<string>{
                    "MAN C", "GLA A", "EDI B"
                }
                , BaseType.DirectedPair, Cardinality.Multiple);

            ResponseProcessor.Process(assessmentItem, assessmentResult, logger);

            var result = assessmentResult.GetScoreForItem(assessmentItem.Identifier, "SCORE");
            Assert.Equal("3", result);
        }


        [Fact]
        public void IMS_ExampleGraphicGapMatchInteractionResponseProcessing_Partly_Correct()
        {
            var logger = new Mock<ILogger>().Object;

            var assessmentItem = new AssessmentItem(logger, XDocument.Load(File.OpenRead("Resources/2x/ResponseProcessing/IMS-examples/graphic_gap_match.xml")), TestHelper.GetExpressionFactory());
            var assessmentResult = TestHelper.GetBasicAssessmentResult();
            assessmentResult.AddCandidateResponses
                (assessmentItem.Identifier, "RESPONSE", new List<string>{
                    "EBG C", "GLA A", "EDI B"
                }
                , BaseType.DirectedPair, Cardinality.Multiple);

            ResponseProcessor.Process(assessmentItem, assessmentResult, logger);

            var result = assessmentResult.GetScoreForItem(assessmentItem.Identifier, "SCORE");
            Assert.Equal("1", result);
        }

        [Fact]
        public void IMS_ExampleGraphicGapMatchInteractionResponseProcessing_Incorrect()
        {
            var logger = new Mock<ILogger>().Object;

            var assessmentItem = new AssessmentItem(logger, XDocument.Load(File.OpenRead("Resources/2x/ResponseProcessing/IMS-examples/graphic_gap_match.xml")), TestHelper.GetExpressionFactory());
            var assessmentResult = TestHelper.GetBasicAssessmentResult();
            assessmentResult.AddCandidateResponses
                (assessmentItem.Identifier, "RESPONSE", new List<string>{
                    "EBG C", "GLA A", "MCH B"
                }
                , BaseType.DirectedPair, Cardinality.Multiple);

            ResponseProcessor.Process(assessmentItem, assessmentResult, logger);

            var result = assessmentResult.GetScoreForItem(assessmentItem.Identifier, "SCORE");
            Assert.Equal("0", result);
        }
        [Fact]
        public void IMS_ExampleGraphicGapMatchInteraction_QTI3_Correct()
        {
            var logger = new Mock<ILogger>().Object;

            var assessmentItem = new AssessmentItem(logger, XDocument.Load(File.OpenRead("Resources/30/ResponseProcessing/IMS-examples/graphic-gap-match-qti3.xml")), TestHelper.GetExpressionFactory());
            var assessmentResult = TestHelper.GetBasicAssessmentResult();
            assessmentResult.AddCandidateResponses
                (assessmentItem.Identifier, "RESPONSE", new List<string>{
                    "DraggerD B", "DraggerC C", "DraggerA D", "DraggerB A"
                }
                , BaseType.DirectedPair, Cardinality.Multiple);
            ResponseProcessor.Process(assessmentItem, assessmentResult, logger);

            var result = assessmentResult.GetScoreForItem(assessmentItem.Identifier, "SCORE");
            Assert.Equal("1", result);
        }

        [Fact]
        public void IMS_ExampleGraphicGapMatchInteraction_QTI3_Incorrect()
        {
            var logger = new Mock<ILogger>().Object;

            var assessmentItem = new AssessmentItem(logger, XDocument.Load(File.OpenRead("Resources/30/ResponseProcessing/IMS-examples/graphic-gap-match-qti3.xml")), TestHelper.GetExpressionFactory());
            var assessmentResult = TestHelper.GetBasicAssessmentResult();
            assessmentResult.AddCandidateResponses
                (assessmentItem.Identifier, "RESPONSE", new List<string>{
                    "DraggerD B", "DraggerC C", "DraggerA A", "DraggerB D"
                }
                , BaseType.DirectedPair, Cardinality.Multiple);
            ResponseProcessor.Process(assessmentItem, assessmentResult, logger);

            var result = assessmentResult.GetScoreForItem(assessmentItem.Identifier, "SCORE");
            Assert.Equal("0", result);
        }
    }
}
