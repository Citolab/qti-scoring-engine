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
    public class AssociateInteractionTests
    {
        //<correctResponse>
        //	<value>A P</value>
        //	<value>C M</value>
        //	<value>D L</value>
        //</correctResponse>
        //<mapping defaultValue="0">
        //	<mapEntry mapKey="A P" mappedValue="2"/>
        //	<mapEntry mapKey="C M" mappedValue="1"/>
        //	<mapEntry mapKey="D L" mappedValue="1"/>
        //</mapping>

        [Fact]
        public void IMS_ExampleAssociateInteractionResponseProcessing_Correct()
        {
            var logger = new Mock<ILogger>().Object;

            var assessmentItem = new AssessmentItem(logger, XDocument.Load(File.OpenRead("Resources/ResponseProcessing/IMS-examples/associate.xml")));
            var assessmentResult = TestHelper.GetBasicAssessmentResult();
            assessmentResult.AddCandidateResponses
                (assessmentItem.Identifier, "RESPONSE", new List<string>{
                    "A P",
                    "C M",
                    "D L" }
                , BaseType.Identifier, Cardinality.Ordered);

            ResponseProcessor.Process(assessmentItem, assessmentResult, logger);

            var result = assessmentResult.GetScoreForItem(assessmentItem.Identifier, "SCORE");
            Assert.Equal("4", result);
        }

        [Fact]
        public void IMS_ExampleAssociateInteractionResponseProcessing_Correct2()
        {
            var logger = new Mock<ILogger>().Object;

            var assessmentItem = new AssessmentItem(logger, XDocument.Load(File.OpenRead("Resources/ResponseProcessing/IMS-examples/associate.xml")));
            var assessmentResult = TestHelper.GetBasicAssessmentResult();
            assessmentResult.AddCandidateResponses
                (assessmentItem.Identifier, "RESPONSE", new List<string>{
                    "P A",
                    "C M",
                    "D L" }
                , BaseType.Identifier, Cardinality.Ordered);

            ResponseProcessor.Process(assessmentItem, assessmentResult, logger);

            var result = assessmentResult.GetScoreForItem(assessmentItem.Identifier, "SCORE");
            Assert.Equal("4", result);
        }

        [Fact]
        public void IMS_ExampleAssociateInteractionResponseProcessing_Partly_Correct()
        {
            var logger = new Mock<ILogger>().Object;

            var assessmentItem = new AssessmentItem(logger, XDocument.Load(File.OpenRead("Resources/ResponseProcessing/IMS-examples/associate.xml")));
            var assessmentResult = TestHelper.GetBasicAssessmentResult();
            assessmentResult.AddCandidateResponses
                (assessmentItem.Identifier, "RESPONSE", new List<string>{
                    "P A",
                    "D L" }
                , BaseType.Identifier, Cardinality.Ordered);

            ResponseProcessor.Process(assessmentItem, assessmentResult, logger);

            var result = assessmentResult.GetScoreForItem(assessmentItem.Identifier, "SCORE");
            Assert.Equal("3", result);
        }

        [Fact]
        public void IMS_ExampleAssociateInteractionResponseProcessing_Incorrect()
        {
            var logger = new Mock<ILogger>().Object;

            var assessmentItem = new AssessmentItem(logger, XDocument.Load(File.OpenRead("Resources/ResponseProcessing/IMS-examples/associate.xml")));
            var assessmentResult = TestHelper.GetBasicAssessmentResult();
            assessmentResult.AddCandidateResponses
                (assessmentItem.Identifier, "RESPONSE", new List<string>{
                    "P C",
                    "D M",
                    "L A"}
                , BaseType.Identifier, Cardinality.Ordered);

            ResponseProcessor.Process(assessmentItem, assessmentResult, logger);

            var result = assessmentResult.GetScoreForItem(assessmentItem.Identifier, "SCORE");
            Assert.Equal("0", result);
        }
    }
}
