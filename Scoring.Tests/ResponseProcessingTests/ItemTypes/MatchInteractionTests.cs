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

    public class MatchInteractionTests
    {
        //<correctResponse>
        //  <value>C R</value>
        //  <value>D M</value>
        //  <value>L M</value>
        //  <value>P T</value>
        //</correctResponse>
        //<mapping defaultValue="0">
        //  <mapEntry mapKey="C R" mappedValue="1"/>
        //  <mapEntry mapKey="D M" mappedValue="0.5"/>
        //  <mapEntry mapKey="L M" mappedValue="0.5"/>
        //  <mapEntry mapKey="P T" mappedValue="1"/>
        //</mapping>

        [Fact]
        public void IMS_ExampleMatchInteractionResponseProcessing_Correct()
        {
            var logger = new Mock<ILogger>().Object;

            var assessmentItem = new AssessmentItem(logger, XDocument.Load(File.OpenRead("Resources/2x/ResponseProcessing/IMS-examples/match.xml")), TestHelper.GetExpressionFactory());
            var assessmentResult = TestHelper.GetBasicAssessmentResult();
            assessmentResult.AddCandidateResponses
                (assessmentItem.Identifier, "RESPONSE", new List<string>{
                    "C R",
                    "D M",
                    "L M",
                    "P T"
                }
                , BaseType.DirectedPair, Cardinality.Multiple);

            ResponseProcessor.Process(assessmentItem, assessmentResult, logger);

            var result = assessmentResult.GetScoreForItem(assessmentItem.Identifier, "SCORE");
            Assert.Equal("3", result);
        }

        [Fact]
        public void IMS_ExampleMatchInteractionResponseProcessing_Incorrect()
        {
            var logger = new Mock<ILogger>().Object;

            var assessmentItem = new AssessmentItem(logger, XDocument.Load(File.OpenRead("Resources/2x/ResponseProcessing/IMS-examples/match.xml")), TestHelper.GetExpressionFactory());
            var assessmentResult = TestHelper.GetBasicAssessmentResult();
            assessmentResult.AddCandidateResponses
                (assessmentItem.Identifier, "RESPONSE", new List<string>{
                    "R C",
                    "D M",
                    "L M",
                    "P T"
                }
                , BaseType.DirectedPair, Cardinality.Multiple);

            ResponseProcessor.Process(assessmentItem, assessmentResult, logger);

            var result = assessmentResult.GetScoreForItem(assessmentItem.Identifier, "SCORE");
            Assert.Equal("2", result);
        }

        [Fact]
        public void IMS_ExampleMatchInteractionResponseProcessing_Incorrect2()
        {
            var logger = new Mock<ILogger>().Object;

            var assessmentItem = new AssessmentItem(logger, XDocument.Load(File.OpenRead("Resources/2x/ResponseProcessing/IMS-examples/match.xml")), TestHelper.GetExpressionFactory());
            var assessmentResult = TestHelper.GetBasicAssessmentResult();
            assessmentResult.AddCandidateResponses
                (assessmentItem.Identifier, "RESPONSE", new List<string>{
                    "D M",
                    "L M",
                    "P T"
                }
                , BaseType.DirectedPair, Cardinality.Multiple);

            ResponseProcessor.Process(assessmentItem, assessmentResult, logger);

            var result = assessmentResult.GetScoreForItem(assessmentItem.Identifier, "SCORE");
            Assert.Equal("2", result);
        }
    }
}
