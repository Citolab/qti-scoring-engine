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
            var assessmentItem = new AssessmentItem(logger, XDocument.Load(File.OpenRead("Resources/2x/ResponseProcessing/1004.xml")), TestHelper.GetExpressionFactory());

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
            var assessmentItem = new AssessmentItem(logger, XDocument.Load(File.OpenRead("Resources/2x/ResponseProcessing/1004.xml")), TestHelper.GetExpressionFactory());

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
            var assessmentItem = new AssessmentItem(logger, XDocument.Load(File.OpenRead("Resources/2x/ResponseProcessing/1004.xml")), TestHelper.GetExpressionFactory());

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
            var assessmentItem = new AssessmentItem(logger, XDocument.Load(File.OpenRead("Resources/2x/ResponseProcessing/1004-CaseInsensitive.xml")), TestHelper.GetExpressionFactory());

            ResponseProcessor.Process(assessmentItem, assessmentResult, logger);

            var scoreValue = assessmentResult.GetScoreForItem("ITM-1004", "SCORE");
            Assert.Equal("1", scoreValue);
        }

        [Fact]
        public void IMS_ExampleChoiceInlineResponseProcessing_Correct()
        {
            var logger = new Mock<ILogger>().Object;

            var assessmentItem = new AssessmentItem(logger, XDocument.Load(File.OpenRead("Resources/2x/ResponseProcessing/IMS-examples/text_entry.xml")), TestHelper.GetExpressionFactory());
            var assessmentResult = TestHelper.GetBasicAssessmentResult();
            assessmentResult.AddCandidateResponse
    (assessmentItem.Identifier, "RESPONSE", "York"
    , BaseType.String, Cardinality.Single);
            ResponseProcessor.Process(assessmentItem, assessmentResult, logger);

            var result = assessmentResult.GetScoreForItem(assessmentItem.Identifier, "SCORE");
            Assert.Equal("1", result);
        }

        [Fact]
        public void IMS_ExampleChoiceInlineResponseProcessing_Partially_Correct()
        {
            var logger = new Mock<ILogger>().Object;

            var assessmentItem = new AssessmentItem(logger, XDocument.Load(File.OpenRead("Resources/2x/ResponseProcessing/IMS-examples/text_entry.xml")), TestHelper.GetExpressionFactory());
            var assessmentResult = TestHelper.GetBasicAssessmentResult();
            assessmentResult.AddCandidateResponse
    (assessmentItem.Identifier, "RESPONSE", "york"
    , BaseType.String, Cardinality.Single);
            ResponseProcessor.Process(assessmentItem, assessmentResult, logger);

            var result = assessmentResult.GetScoreForItem(assessmentItem.Identifier, "SCORE");
            Assert.Equal("0.5", result);
        }

        [Fact]
        public void IMS_ExampleChoiceInlineResponseProcessing_Incorrect()
        {
            var logger = new Mock<ILogger>().Object;

            var assessmentItem = new AssessmentItem(logger, XDocument.Load(File.OpenRead("Resources/2x/ResponseProcessing/IMS-examples/text_entry.xml")), TestHelper.GetExpressionFactory());
            var assessmentResult = TestHelper.GetBasicAssessmentResult();
            assessmentResult.AddCandidateResponse
    (assessmentItem.Identifier, "RESPONSE", "newyork"
    , BaseType.String, Cardinality.Single);
            ResponseProcessor.Process(assessmentItem, assessmentResult, logger);

            var result = assessmentResult.GetScoreForItem(assessmentItem.Identifier, "SCORE");
            Assert.Equal("0", result);
        }

        //CustomOperators
        [Fact]
        public void TextEntryCustomOperatorAsciiCorrect()
        {
            var logger = new Mock<ILogger>().Object;

            var assessmentResult = TestHelper.GetBasicAssessmentResult();
            var assessmentItem = new AssessmentItem(logger, XDocument.Load(File.OpenRead("Resources/2x/ResponseProcessing/CustomOperators.xml")), TestHelper.GetExpressionFactory());
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
            var assessmentItem = new AssessmentItem(logger, XDocument.Load(File.OpenRead("Resources/2x/ResponseProcessing/CustomOperators.xml")), TestHelper.GetExpressionFactory());
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
            var assessmentItem = new AssessmentItem(logger, XDocument.Load(File.OpenRead("Resources/2x/ResponseProcessing/CustomOperators.xml")), TestHelper.GetExpressionFactory());
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
            var assessmentItem = new AssessmentItem(logger, XDocument.Load(File.OpenRead("Resources/2x/ResponseProcessing/CustomOperators.xml")), TestHelper.GetExpressionFactory());
            assessmentResult.AddCandidateResponse("ITM-1", "RESPONSE", " fout ", BaseType.String, Cardinality.Single);

            ResponseProcessor.Process(assessmentItem, assessmentResult, logger);

            var scoreValue = assessmentResult.GetScoreForItem("ITM-1", "SCORE");
            Assert.Equal("0", scoreValue);
        }
    }
}
