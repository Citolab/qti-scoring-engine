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

    public class GapMatchInteractionTests
    {
        //<correctResponse>
        //  <value>W G1</value>
        //  <value>Su G2</value>
        //</correctResponse>
        //<mapping defaultValue="-1" lowerBound="0">
        //  <mapEntry mapKey="W G1" mappedValue="1"/>
        //  <mapEntry mapKey="Su G2" mappedValue="2"/>
        //</mapping>

        [Fact]
        public void IMS_ExampleMatchInteractionResponseProcessing_Correct()
        {
            var logger = new Mock<ILogger>().Object;

            var assessmentItem = new AssessmentItem(logger, XDocument.Load(File.OpenRead("Resources/ResponseProcessing/IMS-examples/gap_match.xml")));
            var assessmentResult = TestHelper.GetBasicAssessmentResult();
            assessmentResult.AddCandidateResponses
                (assessmentItem.Identifier, "RESPONSE", new List<string>{
                    "W G1",
                    "Su G2"
                }
                , BaseType.DirectedPair, Cardinality.Multiple);

            ResponseProcessor.Process(assessmentItem, assessmentResult, logger);

            var result = assessmentResult.GetScoreForItem(assessmentItem.Identifier, "SCORE");
            Assert.Equal("3", result);
        }
        [Fact]
        public void IMS_ExampleMatchInteractionResponseProcessing_Party_Correct()
        {
            var logger = new Mock<ILogger>().Object;

            var assessmentItem = new AssessmentItem(logger, XDocument.Load(File.OpenRead("Resources/ResponseProcessing/IMS-examples/gap_match.xml")));
            var assessmentResult = TestHelper.GetBasicAssessmentResult();
            assessmentResult.AddCandidateResponses
                (assessmentItem.Identifier, "RESPONSE", new List<string>{
                    "Sp G1",
                    "Su G2"
                }
                , BaseType.DirectedPair, Cardinality.Multiple);

            ResponseProcessor.Process(assessmentItem, assessmentResult, logger);

            var result = assessmentResult.GetScoreForItem(assessmentItem.Identifier, "SCORE");
            Assert.Equal("1", result);
        }
        [Fact]
        public void IMS_ExampleMatchInteractionResponseProcessing_Missing()
        {
            var logger = new Mock<ILogger>().Object;

            var assessmentItem = new AssessmentItem(logger, XDocument.Load(File.OpenRead("Resources/ResponseProcessing/IMS-examples/gap_match.xml")));
            var assessmentResult = TestHelper.GetBasicAssessmentResult();


            ResponseProcessor.Process(assessmentItem, assessmentResult, logger);

            var result = assessmentResult.GetScoreForItem(assessmentItem.Identifier, "SCORE");
            Assert.Equal("0", result);
        }

        [Fact]
        public void IMS_ExampleMatchInteractionResponseProcessing_Incorrect()
        {
            var logger = new Mock<ILogger>().Object;

            var assessmentItem = new AssessmentItem(logger, XDocument.Load(File.OpenRead("Resources/ResponseProcessing/IMS-examples/gap_match.xml")));
            var assessmentResult = TestHelper.GetBasicAssessmentResult();
            assessmentResult.AddCandidateResponses
    (assessmentItem.Identifier, "RESPONSE", new List<string>{
                    "A G1",
                    "Sp G2"
    }
    , BaseType.DirectedPair, Cardinality.Multiple);

            ResponseProcessor.Process(assessmentItem, assessmentResult, logger);

            var result = assessmentResult.GetScoreForItem(assessmentItem.Identifier, "SCORE");
            Assert.Equal("0", result);
        }
    }
}
