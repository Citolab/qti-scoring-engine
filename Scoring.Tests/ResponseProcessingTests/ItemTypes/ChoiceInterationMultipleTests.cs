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

namespace Citolab.QTI.ScoringEngine.Tests.ResponseProcessingTests
{
    public class ChoiceInterationMultipleTests
    {
        [Fact]
        public void ChoiceInteractionMultipleCorrect()
        {
            var logger = new Mock<ILogger>().Object;

            var assessmentItem = new AssessmentItem(logger, XDocument.Load(File.OpenRead("Resources/2x/ResponseProcessing/112505-MR.xml")));
            var assessmentResult = TestHelper.GetBasicAssessmentResult();
            assessmentResult.AddCandidateResponses(assessmentItem.Identifier, "RESPONSE", new List<string> { "D", "E", "F"}, BaseType.Identifier, Cardinality.Multiple);

            ResponseProcessor.Process(assessmentItem, assessmentResult, logger);

            var scoreValue = assessmentResult.GetScoreForItem(assessmentItem.Identifier, "SCORE");
            Assert.Equal("1", scoreValue);
        }

        [Fact]
        public void ChoiceInteractionMultipleIncorrect()
        {
            var logger = new Mock<ILogger>().Object;

            var assessmentItem = new AssessmentItem(logger, XDocument.Load(File.OpenRead("Resources/2x/ResponseProcessing/112505-MR.xml")));
            var assessmentResult = TestHelper.GetBasicAssessmentResult();
            assessmentResult.AddCandidateResponses(assessmentItem.Identifier, "RESPONSE", new List<string> { "E", "F" }, BaseType.Identifier, Cardinality.Multiple);

            ResponseProcessor.Process(assessmentItem, assessmentResult, logger);

            var scoreValue = assessmentResult.GetScoreForItem(assessmentItem.Identifier, "SCORE");
            Assert.Equal("0", scoreValue);
        }

        [Fact]
        public void ChoiceInteractionMultipleIncorrect2()
        {
            var logger = new Mock<ILogger>().Object;

            var assessmentItem = new AssessmentItem(logger, XDocument.Load(File.OpenRead("Resources/2x/ResponseProcessing/112505-MR.xml")));
            var assessmentResult = TestHelper.GetBasicAssessmentResult();
            assessmentResult.AddCandidateResponses(assessmentItem.Identifier, "RESPONSE", new List<string> { "A", "D", "E", "F" }, BaseType.Identifier, Cardinality.Multiple);

            ResponseProcessor.Process(assessmentItem, assessmentResult, logger);

            var scoreValue = assessmentResult.GetScoreForItem(assessmentItem.Identifier, "SCORE");
            Assert.Equal("0", scoreValue);
        }

        [Fact]
        public void ChoiceInteractionMultipleIncorrect3()
        {
            var logger = new Mock<ILogger>().Object;

            var assessmentItem = new AssessmentItem(logger, XDocument.Load(File.OpenRead("Resources/2x/ResponseProcessing/112505-MR.xml")));
            var assessmentResult = TestHelper.GetBasicAssessmentResult();
            assessmentResult.AddCandidateResponses(assessmentItem.Identifier, "RESPONSE", new List<string> {  }, BaseType.Identifier, Cardinality.Multiple);

            ResponseProcessor.Process(assessmentItem, assessmentResult, logger);

            var scoreValue = assessmentResult.GetScoreForItem(assessmentItem.Identifier, "SCORE");
            Assert.Equal("0", scoreValue);
        }

        [Fact]
        public void ChoiceInteractionMultipleImsSiteCorrect()
        {
            var logger = new Mock<ILogger>().Object;

            var assessmentItem = new AssessmentItem(logger, XDocument.Load(File.OpenRead("Resources/2x/ResponseProcessing/IMS-examples/choice_multiple.xml")));
            var assessmentResult = TestHelper.GetBasicAssessmentResult();
            assessmentResult.AddCandidateResponses(assessmentItem.Identifier, "RESPONSE", new List<string> { "H", "O" }, BaseType.Identifier, Cardinality.Multiple);

            ResponseProcessor.Process(assessmentItem, assessmentResult, logger);

            var scoreValue = assessmentResult.GetScoreForItem(assessmentItem.Identifier, "SCORE");
            Assert.Equal("2", scoreValue);
        }

        [Fact]
        public void ChoiceInteractionMultipleImsSitePartly1()
        {
            var logger = new Mock<ILogger>().Object;

            var assessmentItem = new AssessmentItem(logger, XDocument.Load(File.OpenRead("Resources/2x/ResponseProcessing/IMS-examples/choice_multiple.xml")));
            var assessmentResult = TestHelper.GetBasicAssessmentResult();
            assessmentResult.AddCandidateResponses(assessmentItem.Identifier, "RESPONSE", new List<string> { "O" }, BaseType.Identifier, Cardinality.Multiple);

            ResponseProcessor.Process(assessmentItem, assessmentResult, logger);

            var scoreValue = assessmentResult.GetScoreForItem(assessmentItem.Identifier, "SCORE");
            Assert.Equal("1", scoreValue);
        }

        [Fact]
        public void ChoiceInteractionMultipleImsSitePartly2()
        {
            var logger = new Mock<ILogger>().Object;

            var assessmentItem = new AssessmentItem(logger, XDocument.Load(File.OpenRead("Resources/2x/ResponseProcessing/IMS-examples/choice_multiple.xml")));
            var assessmentResult = TestHelper.GetBasicAssessmentResult();
            assessmentResult.AddCandidateResponses(assessmentItem.Identifier, "RESPONSE", new List<string> { "Cl", "H", "O" }, BaseType.Identifier, Cardinality.Multiple);

            ResponseProcessor.Process(assessmentItem, assessmentResult, logger);

            var scoreValue = assessmentResult.GetScoreForItem(assessmentItem.Identifier, "SCORE");
            Assert.Equal("1", scoreValue);
        }
        [Fact]
        public void ChoiceInteractionMultipleImsSiteIncorrect()
        {
            var logger = new Mock<ILogger>().Object;

            var assessmentItem = new AssessmentItem(logger, XDocument.Load(File.OpenRead("Resources/2x/ResponseProcessing/IMS-examples/choice_multiple.xml")));
            var assessmentResult = TestHelper.GetBasicAssessmentResult();
            assessmentResult.AddCandidateResponses(assessmentItem.Identifier, "RESPONSE", new List<string> { "He", "Cl", "H", "O" }, BaseType.Identifier, Cardinality.Multiple);

            ResponseProcessor.Process(assessmentItem, assessmentResult, logger);

            var scoreValue = assessmentResult.GetScoreForItem(assessmentItem.Identifier, "SCORE");
            Assert.Equal("0", scoreValue);
        }
    }
}
