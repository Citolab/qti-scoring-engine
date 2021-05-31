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
    public class TextEntryFloatTests
    {
        [Fact]
        public void TextEntryCorrect()
        {
            var logger = new Mock<ILogger>().Object;

            var assessmentResult = TestHelper.GetBasicAssessmentResult();
            assessmentResult.AddCandidateResponse("ITM-210091", "RESPONSE", "0.63", BaseType.Float, Cardinality.Single);
            var assessmentItem = new AssessmentItem(logger, XDocument.Load(File.OpenRead("Resources\\ResponseProcessing\\210091.xml")));

            ResponseProcessor.Process(assessmentItem, assessmentResult, logger);

            var scoreValue = assessmentResult.GetScoreForItem("ITM-210091", "SCORE");
            Assert.Equal("1", scoreValue);
        }

        [Fact]
        public void TextEntryIncorrect()
        {
            var logger = new Mock<ILogger>().Object;

            var assessmentResult = TestHelper.GetBasicAssessmentResult();
            assessmentResult.AddCandidateResponse("ITM-210091", "RESPONSE", "0.65", BaseType.Float, Cardinality.Single);
            var assessmentItem = new AssessmentItem(logger, XDocument.Load(File.OpenRead("Resources\\ResponseProcessing\\210091.xml")));

            ResponseProcessor.Process(assessmentItem, assessmentResult, logger);

            var scoreValue = assessmentResult.GetScoreForItem("ITM-210091", "SCORE");
            Assert.Equal("0", scoreValue);
        }

        [Fact]
        public void TextEntryCorrectCustomInteractionComma()
        {
            var logger = new Mock<ILogger>().Object;

            var assessmentResult = TestHelper.GetBasicAssessmentResult();
            assessmentResult.AddCandidateResponse("ITM-210091", "RESPONSE", "0,63", BaseType.Float, Cardinality.Single);
            var assessmentItem = new AssessmentItem(logger, XDocument.Load(File.OpenRead("Resources\\ResponseProcessing\\210091-customOperator.xml")));

            ResponseProcessor.Process(assessmentItem, assessmentResult, logger);

            var scoreValue = assessmentResult.GetScoreForItem("ITM-210091", "SCORE");
            Assert.Equal("1", scoreValue);
        }

        [Fact]
        public void TextEntryCorrectCustomInteractionPoint()
        {
            var logger = new Mock<ILogger>().Object;

            var assessmentResult = TestHelper.GetBasicAssessmentResult();
            assessmentResult.AddCandidateResponse("ITM-210091", "RESPONSE", "0.63", BaseType.Float, Cardinality.Single);
            var assessmentItem = new AssessmentItem(logger, XDocument.Load(File.OpenRead("Resources\\ResponseProcessing\\210091-customOperator.xml")));

            ResponseProcessor.Process(assessmentItem, assessmentResult, logger);

            var scoreValue = assessmentResult.GetScoreForItem("ITM-210091", "SCORE");
            Assert.Equal("1", scoreValue);
        }
        [Fact]
        public void TextEntryCorrectCustomInteractionComma2()
        {
            var logger = new Mock<ILogger>().Object;

            var assessmentResult = TestHelper.GetBasicAssessmentResult();
            assessmentResult.AddCandidateResponse("ITM-210091", "RESPONSE", "0,6300", BaseType.Float, Cardinality.Single);
            var assessmentItem = new AssessmentItem(logger, XDocument.Load(File.OpenRead("Resources\\ResponseProcessing\\210091-customOperator.xml")));

            ResponseProcessor.Process(assessmentItem, assessmentResult, logger);

            var scoreValue = assessmentResult.GetScoreForItem("ITM-210091", "SCORE");
            Assert.Equal("1", scoreValue);
        }

        [Fact]
        public void TextEntryIncorrectCustomInteractionComma()
        {
            var logger = new Mock<ILogger>().Object;

            var assessmentResult = TestHelper.GetBasicAssessmentResult();
            assessmentResult.AddCandidateResponse("ITM-210091", "RESPONSE", "0,6301", BaseType.Float, Cardinality.Single);
            var assessmentItem = new AssessmentItem(logger, XDocument.Load(File.OpenRead("Resources\\ResponseProcessing\\210091-customOperator.xml")));

            ResponseProcessor.Process(assessmentItem, assessmentResult, logger);

            var scoreValue = assessmentResult.GetScoreForItem("ITM-210091", "SCORE");
            Assert.Equal("0", scoreValue);
        }
    }
}
