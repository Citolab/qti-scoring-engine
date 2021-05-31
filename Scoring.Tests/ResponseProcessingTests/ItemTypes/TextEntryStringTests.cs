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
    public class TextEntryStringTests
    {
        [Fact]
        public void TextEntryCorrect()
        {
            var logger = new Mock<ILogger>().Object;

            var assessmentResult = TestHelper.GetBasicAssessmentResult();
            assessmentResult.AddCandidateResponse("ITM-1004", "RESPONSE", "rek", BaseType.String, Cardinality.Single);
            var assessmentItem = new AssessmentItem(logger, XDocument.Load(File.OpenRead("Resources/ResponseProcessing/1004.xml")));

            ResponseProcessor.Process(assessmentItem, assessmentResult, logger);

            var scoreValue = assessmentResult.GetScoreForItem("ITM-1004", "SCORE");
            Assert.Equal("1", scoreValue);
        }

        [Fact]
        public void TextEntryIncorrect()
        {
            var logger = new Mock<ILogger>().Object;

            var assessmentResult = TestHelper.GetBasicAssessmentResult();
            assessmentResult.AddCandidateResponse("ITM-1004", "RESPONSE", "fout", BaseType.String, Cardinality.Single);
            var assessmentItem = new AssessmentItem(logger, XDocument.Load(File.OpenRead("Resources/ResponseProcessing/1004.xml")));

            ResponseProcessor.Process(assessmentItem, assessmentResult, logger);

            var scoreValue = assessmentResult.GetScoreForItem("ITM-1004", "SCORE");
            Assert.Equal("0", scoreValue);
        }

        [Fact]
        public void TextEntryIncorrectCasing()
        {
            var logger = new Mock<ILogger>().Object;

            var assessmentResult = TestHelper.GetBasicAssessmentResult();
            assessmentResult.AddCandidateResponse("ITM-1004", "RESPONSE", "rEK", BaseType.String, Cardinality.Single);
            var assessmentItem = new AssessmentItem(logger, XDocument.Load(File.OpenRead("Resources/ResponseProcessing/1004.xml")));

            ResponseProcessor.Process(assessmentItem, assessmentResult, logger);

            var scoreValue = assessmentResult.GetScoreForItem("ITM-1004", "SCORE");
            Assert.Equal("0", scoreValue);
        }

        [Fact]
        public void TextEntryCorrectCasing()
        {
            var logger = new Mock<ILogger>().Object;

            var assessmentResult = TestHelper.GetBasicAssessmentResult();
            assessmentResult.AddCandidateResponse("ITM-1004", "RESPONSE", "rEK", BaseType.String, Cardinality.Single);
            var assessmentItem = new AssessmentItem(logger, XDocument.Load(File.OpenRead("Resources/ResponseProcessing/1004-caseInsensitive.xml")));

            ResponseProcessor.Process(assessmentItem, assessmentResult, logger);

            var scoreValue = assessmentResult.GetScoreForItem("ITM-1004", "SCORE");
            Assert.Equal("1", scoreValue);
        }

        //CustomOperators
        [Fact]
        public void TextEntryCustomOperatorAsciiCorrect()
        {
            var logger = new Mock<ILogger>().Object;

            var assessmentResult = TestHelper.GetBasicAssessmentResult();
            var assessmentItem = new AssessmentItem(logger, XDocument.Load(File.OpenRead("Resources/ResponseProcessing/CustomOperators.xml")));
            assessmentResult.AddCandidateResponse("ITM-1", "RESPONSE", "tést", BaseType.String, Cardinality.Single);

            ResponseProcessor.Process(assessmentItem, assessmentResult, logger);

            var scoreValue = assessmentResult.GetScoreForItem("ITM-1", "SCORE");
            Assert.Equal("1", scoreValue);
        }

        [Fact]
        public void TextEntryCustomOperatorAsciiCorrect2()
        {
            var logger = new Mock<ILogger>().Object;

            var assessmentResult = TestHelper.GetBasicAssessmentResult();
            var assessmentItem = new AssessmentItem(logger, XDocument.Load(File.OpenRead("Resources/ResponseProcessing/CustomOperators.xml")));
            assessmentResult.AddCandidateResponse("ITM-1", "RESPONSE", "tḝst", BaseType.String, Cardinality.Single);

            ResponseProcessor.Process(assessmentItem, assessmentResult, logger);

            var scoreValue = assessmentResult.GetScoreForItem("ITM-1", "SCORE");
            Assert.Equal("1", scoreValue);
        }

        [Fact]
        public void TextEntryCustomOperatorAsciiAndTrimCorrect()
        {
            var logger = new Mock<ILogger>().Object;

            var assessmentResult = TestHelper.GetBasicAssessmentResult();
            var assessmentItem = new AssessmentItem(logger, XDocument.Load(File.OpenRead("Resources/ResponseProcessing/CustomOperators.xml")));
            assessmentResult.AddCandidateResponse("ITM-1", "RESPONSE", " tḝst ", BaseType.String, Cardinality.Single);

            ResponseProcessor.Process(assessmentItem, assessmentResult, logger);

            var scoreValue = assessmentResult.GetScoreForItem("ITM-1", "SCORE");
            Assert.Equal("1", scoreValue);
        }

        [Fact]
        public void TextEntryCustomOperatorAsciiAndTrimIncorrect()
        {
            var logger = new Mock<ILogger>().Object;

            var assessmentResult = TestHelper.GetBasicAssessmentResult();
            var assessmentItem = new AssessmentItem(logger, XDocument.Load(File.OpenRead("Resources/ResponseProcessing/CustomOperators.xml")));
            assessmentResult.AddCandidateResponse("ITM-1", "RESPONSE", " fout ", BaseType.String, Cardinality.Single);

            ResponseProcessor.Process(assessmentItem, assessmentResult, logger);

            var scoreValue = assessmentResult.GetScoreForItem("ITM-1", "SCORE");
            Assert.Equal("0", scoreValue);
        }
    }
}
