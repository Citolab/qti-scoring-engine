using Citolab.QTI.ScoringEngine.Model;
using Citolab.QTI.ScoringEngine.ResponseProcessing;
using Citolab.QTI.ScoringEngine.Tests;
using Microsoft.Extensions.Logging;
using Moq;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;
using Xunit;
namespace ScoringEngine.Tests.ResponseProcessingTests.ItemTypes
{
   public  class SelectPointTests
    {

        [Fact]
        public void IMS_ExampleSelectPointResponseProcessing_Correct()
        {
            var logger = new Mock<ILogger>().Object;

            var assessmentItem = new AssessmentItem(logger, XDocument.Load(File.OpenRead("Resources/ResponseProcessing/IMS-examples/select_point.xml")));
            var assessmentResult = TestHelper.GetBasicAssessmentResult();

            var firstMappping = assessmentItem.ResponseDeclarations.Values.First(r => r.AreaMapping != null).AreaMapping.AreaMappings.First();
            var correctPoint = firstMappping.GetCorrectResponse();

            var response = $"{correctPoint.Value.X} {correctPoint.Value.Y}";
          
            assessmentResult.AddCandidateResponse
                (assessmentItem.Identifier, "RESPONSE", response, BaseType.Point, Cardinality.Single);

            ResponseProcessor.Process(assessmentItem, assessmentResult, logger);

            var result = assessmentResult.GetScoreForItem(assessmentItem.Identifier, "SCORE");
            Assert.Equal("1", result);
        }

        [Fact]
        public void IMS_ExampleSelectPointResponseProcessing_InCorrect()
        {
            var logger = new Mock<ILogger>().Object;

            var assessmentItem = new AssessmentItem(logger, XDocument.Load(File.OpenRead("Resources/ResponseProcessing/IMS-examples/select_point.xml")));
            var assessmentResult = TestHelper.GetBasicAssessmentResult();

            var firstMappping = assessmentItem.ResponseDeclarations.Values.First(r => r.AreaMapping != null).AreaMapping.AreaMappings.First();;

            var response = $"0 0";

            assessmentResult.AddCandidateResponse
                (assessmentItem.Identifier, "RESPONSE", response, BaseType.Point, Cardinality.Single);

            ResponseProcessor.Process(assessmentItem, assessmentResult, logger);

            var result = assessmentResult.GetScoreForItem(assessmentItem.Identifier, "SCORE");
            Assert.Equal("0", result);
        }
    }
}
