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
    //  <value>16</value>
    //</correctResponse>
    //<mapping defaultValue="0">
    //  <mapEntry mapKey="12" mappedValue="0.5"/>
    //  <mapEntry mapKey="13" mappedValue="0.5"/>
    //  <mapEntry mapKey="14" mappedValue="1.0"/>
    //  <mapEntry mapKey="15" mappedValue="1.0"/>
    //  <mapEntry mapKey="16" mappedValue="1.0"/>
    //  <mapEntry mapKey="17" mappedValue="1.0"/>
    //  <mapEntry mapKey="18" mappedValue="1.0"/>
    //  <mapEntry mapKey="19" mappedValue="0.5"/>
    //  <mapEntry mapKey="20" mappedValue="0.5"/>
    //</mapping>
   public class SliderInteractionTests
    {

        [Fact]
        public void IMS_ExampleSliderResponseProcessing_Correct()
        {
            var logger = new Mock<ILogger>().Object;

            var assessmentItem = new AssessmentItem(logger, XDocument.Load(File.OpenRead("Resources/2x/ResponseProcessing/IMS-examples/slider.xml")), TestHelper.GetExpressionFactory());
            var assessmentResult = TestHelper.GetBasicAssessmentResult();
            assessmentResult.AddCandidateResponse
                (assessmentItem.Identifier, "RESPONSE", "16"
                , BaseType.Int, Cardinality.Single);

            ResponseProcessor.Process(assessmentItem, assessmentResult, logger);

            var result = assessmentResult.GetScoreForItem(assessmentItem.Identifier, "SCORE");
            Assert.Equal("1", result);
        }

        [Fact]
        public void IMS_ExampleSliderResponseProcessing_Partly_Correct()
        {
            var logger = new Mock<ILogger>().Object;

            var assessmentItem = new AssessmentItem(logger, XDocument.Load(File.OpenRead("Resources/2x/ResponseProcessing/IMS-examples/slider.xml")), TestHelper.GetExpressionFactory());
            var assessmentResult = TestHelper.GetBasicAssessmentResult();
            assessmentResult.AddCandidateResponse
                (assessmentItem.Identifier, "RESPONSE", "20"
                , BaseType.Int, Cardinality.Single);

            ResponseProcessor.Process(assessmentItem, assessmentResult, logger);

            var result = assessmentResult.GetScoreForItem(assessmentItem.Identifier, "SCORE");
            Assert.Equal("0.5", result);
        }

        [Fact]
        public void IMS_ExampleSliderResponseProcessing_Partly_Incorrect()
        {
            var logger = new Mock<ILogger>().Object;

            var assessmentItem = new AssessmentItem(logger, XDocument.Load(File.OpenRead("Resources/2x/ResponseProcessing/IMS-examples/slider.xml")), TestHelper.GetExpressionFactory());
            var assessmentResult = TestHelper.GetBasicAssessmentResult();
            assessmentResult.AddCandidateResponse
                (assessmentItem.Identifier, "RESPONSE", "21"
                , BaseType.Int, Cardinality.Single);

            ResponseProcessor.Process(assessmentItem, assessmentResult, logger);

            var result = assessmentResult.GetScoreForItem(assessmentItem.Identifier, "SCORE");
            Assert.Equal("0", result);
        }
    }
}
