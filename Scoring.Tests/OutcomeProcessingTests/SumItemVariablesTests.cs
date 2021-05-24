using Microsoft.Extensions.Logging;
using Moq;
using Citolab.QTI.ScoringEngine.Model;
using Citolab.QTI.ScoringEngine.OutcomeProcessing;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;
using Xunit;

namespace Scoring.Tests.OutcomeProcessingTests
{
    public class SumItemVariablesTests
    {
        //Test_Toets_Multiple_SubSections
        [Fact]
        public void Outcome_All_Correct()
        {
            //arrange - act
            var assessmentResult = TestHelper.StartOutcomeProcessing("Test_Toets2", "AssessmentResult_Correct");
            //assert
            Assert.Equal("2", assessmentResult.GetScoreForTest("TST-Test_toets", "SCORE"));
        }

        [Fact]
        public void Outcome_All_Correct_Weight()
        {
            //arrange - act
            var assessmentResult = TestHelper.StartOutcomeProcessing("Test_toets2-weight", "AssessmentResult_Correct");
            //assert
            Assert.Equal("1", assessmentResult.GetScoreForTest("TST-Test_toets", "SCORE"));
        }


        [Fact]
        public void Outcome_All_Incorrect_Weight()
        {
            //arrange
            var mockLogger = new Mock<ILogger>();
            var logger = mockLogger.Object;
            var assessmentTest = new AssessmentTest(logger, XDocument.Load(File.OpenRead($"Resources\\OutcomeProcessing\\Test_toets2-weight.xml")));
            var assessmentResult = new AssessmentResult(logger, XDocument.Load(File.OpenRead($"Resources\\OutcomeProcessing\\AssessmentResult_Correct.xml")));
            assessmentResult.ChangeItemResult("ITM-SN02945", "0");
            assessmentResult.ChangeItemResult("ITM-SN02946", "0");
            var outcomeProcessing = new OutcomeProcessing();
            // actt
            outcomeProcessing.Process(assessmentTest, assessmentResult, logger);
            //assert
            Assert.Equal("0", assessmentResult.GetScoreForTest("TST-Test_toets", "SCORE"));
        }

        [Fact]
        public void Outcome_All_Incorrect()
        {
            //arrange
            var mockLogger = new Mock<ILogger>();
            var logger = mockLogger.Object;
            var assessmentTest = new AssessmentTest(logger, XDocument.Load(File.OpenRead($"Resources\\OutcomeProcessing\\Test_toets2.xml")));
            var assessmentResult = new AssessmentResult(logger, XDocument.Load(File.OpenRead($"Resources\\OutcomeProcessing\\AssessmentResult_Correct.xml")));
            assessmentResult.ChangeItemResult("ITM-SN02945", "0");
            assessmentResult.ChangeItemResult("ITM-SN02946", "0");
            var outcomeProcessing = new OutcomeProcessing();
            // actt
            outcomeProcessing.Process(assessmentTest, assessmentResult, logger);
            //assert
            Assert.Equal("0", assessmentResult.GetScoreForTest("TST-Test_toets", "SCORE"));
        }

        [Fact]
        public void Outcome_All_CorrectAndInCorrect()
        {
            //arrange
            var mockLogger = new Mock<ILogger>();
            var logger = mockLogger.Object;
            var assessmentTest = new AssessmentTest(logger, XDocument.Load(File.OpenRead($"Resources\\OutcomeProcessing\\Test_toets2.xml")));
            var assessmentResult = new AssessmentResult(logger, XDocument.Load(File.OpenRead($"Resources\\OutcomeProcessing\\AssessmentResult_Correct.xml")));
            assessmentResult.ChangeItemResult("ITM-SN02945", "0");
            assessmentResult.ChangeItemResult("ITM-SN02946", "1");
            var outcomeProcessing = new OutcomeProcessing();
            // actt
            outcomeProcessing.Process(assessmentTest, assessmentResult, logger);
            //assert
            Assert.Equal("1", assessmentResult.GetScoreForTest("TST-Test_toets", "SCORE"));
        }

        [Fact]
        public void Outcome_All_CorrectAndInCorrect_Weight()
        {
            //arrange
            var mockLogger = new Mock<ILogger>();
            var logger = mockLogger.Object;
            var assessmentTest = new AssessmentTest(logger, XDocument.Load(File.OpenRead($"Resources\\OutcomeProcessing\\Test_toets2-weight.xml")));
            var assessmentResult = new AssessmentResult(logger, XDocument.Load(File.OpenRead($"Resources\\OutcomeProcessing\\AssessmentResult_Correct.xml")));
            assessmentResult.ChangeItemResult("ITM-SN02945", "0");
            assessmentResult.ChangeItemResult("ITM-SN02946", "1");
            var outcomeProcessing = new OutcomeProcessing();
            // actt
            outcomeProcessing.Process(assessmentTest, assessmentResult, logger);
            //assert
            Assert.Equal("0", assessmentResult.GetScoreForTest("TST-Test_toets", "SCORE"));
        }
    }


}
