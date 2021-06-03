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
    //  <value>118 184</value>
    //  <value>150 235</value>
    //  <value>96 114</value>
    //</correctResponse>
    //<areaMapping defaultValue="0">
    //  <areaMapEntry shape="circle" coords="118,184,12" mappedValue="1"/>
    //  <areaMapEntry shape="circle" coords="150,235,12" mappedValue="1"/>
    //  <areaMapEntry shape="circle" coords="96,114,12" mappedValue="1"/>
    //</areaMapping>
    public class PositionObjectInteractionTests
    {
        [Fact]
        public void IMS_ExamplePositionObjectResponseProcessing_Correct()
        {
            var logger = new Mock<ILogger>().Object;

            var assessmentItem = new AssessmentItem(logger, XDocument.Load(File.OpenRead("Resources/2x/ResponseProcessing/IMS-examples/position_object.xml")));
            var assessmentResult = TestHelper.GetBasicAssessmentResult();

            var responses = assessmentItem.ResponseDeclarations.Values.First(r => r.AreaMapping != null)
                    .AreaMapping.AreaMappings.Select(areaMapping =>
                    {
                        var correctPoint =  areaMapping.GetCorrectResponse();
                        return $"{correctPoint.Value.X} {correctPoint.Value.Y}";
                    })
                    .ToList();


            assessmentResult.AddCandidateResponses
                (assessmentItem.Identifier, "RESPONSE", responses, BaseType.Point, Cardinality.Multiple);

            ResponseProcessor.Process(assessmentItem, assessmentResult, logger);

            var result = assessmentResult.GetScoreForItem(assessmentItem.Identifier, "SCORE");
            Assert.Equal("3", result);
        }

        [Fact]
        public void IMS_ExamplePositionObjectProcessing_Partly_Correct()
        {
            var logger = new Mock<ILogger>().Object;

            var assessmentItem = new AssessmentItem(logger, XDocument.Load(File.OpenRead("Resources/2x/ResponseProcessing/IMS-examples/position_object.xml")));
            var assessmentResult = TestHelper.GetBasicAssessmentResult();

            var responses = assessmentItem.ResponseDeclarations.Values.First(r => r.AreaMapping != null)
                    .AreaMapping.AreaMappings.Select(areaMapping =>
                    {
                        var correctPoint = areaMapping.GetCorrectResponse();
                        return $"{correctPoint.Value.X} {correctPoint.Value.Y}";
                    })
                    .Take(2)
                    .ToList();


            assessmentResult.AddCandidateResponses
                (assessmentItem.Identifier, "RESPONSE", responses, BaseType.Point, Cardinality.Multiple);

            ResponseProcessor.Process(assessmentItem, assessmentResult, logger);

            var result = assessmentResult.GetScoreForItem(assessmentItem.Identifier, "SCORE");
            Assert.Equal("2", result);
        }

        [Fact]
        public void IMS_ExamplePositionObjectResponseProcessing_Incorrect()
        {
            var logger = new Mock<ILogger>().Object;

            var assessmentItem = new AssessmentItem(logger, XDocument.Load(File.OpenRead("Resources/2x/ResponseProcessing/IMS-examples/position_object.xml")));
            var assessmentResult = TestHelper.GetBasicAssessmentResult();

            var responses = assessmentItem.ResponseDeclarations.Values.First(r => r.AreaMapping != null)
                    .AreaMapping.AreaMappings.Select(areaMapping =>
                    {
                        return $"0 0";
                    })
                    .ToList();


            assessmentResult.AddCandidateResponses
                (assessmentItem.Identifier, "RESPONSE", responses, BaseType.Point, Cardinality.Multiple);

            ResponseProcessor.Process(assessmentItem, assessmentResult, logger);

            var result = assessmentResult.GetScoreForItem(assessmentItem.Identifier, "SCORE");
            Assert.Equal("0", result);
        }

        }
}
