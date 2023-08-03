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

namespace Citolab.QTI.ScoringEngine.Tests.OutcomeProcessingTests
{
    public class SumItemVariablesTests
    {
        //Test_Toets_Multiple_Subsections
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
            var assessmentResult = TestHelper.StartOutcomeProcessing("Test_Toets2-Weight", "AssessmentResult_Correct");
            //assert
            Assert.Equal("1", assessmentResult.GetScoreForTest("TST-Test_toets", "SCORE"));
        }

        [Fact]
        public void Outcome_All_Correct_Weight_Float()
        {
            //arrange
            var mockLogger = new Mock<ILogger>();
            var logger = mockLogger.Object;
            var assessmentTest = new AssessmentTest(logger, XDocument.Load(File.OpenRead($"Resources/2x/OutcomeProcessing/Test_Toets_Weight_Float.xml")), TestHelper.GetExpressionFactory());
            var assessmentResult = new AssessmentResult(logger, XDocument.Load(File.OpenRead($"Resources/2x/OutcomeProcessing/AssessmentResult_Correct.xml")));
            // actt
            OutcomeProcessor.Process(assessmentTest, assessmentResult, logger);
            Assert.Equal("2.5", assessmentResult.GetScoreForTest("TST-Test_toets", "SCORE"));
        }


        [Fact]
        public void Outcome_All_Incorrect_Weight()
        {
            //arrange
            var mockLogger = new Mock<ILogger>();
            var logger = mockLogger.Object;
            var assessmentTest = new AssessmentTest(logger, XDocument.Load(File.OpenRead($"Resources/2x/OutcomeProcessing/Test_Toets2-Weight.xml")), TestHelper.GetExpressionFactory());
            var assessmentResult = new AssessmentResult(logger, XDocument.Load(File.OpenRead($"Resources/2x/OutcomeProcessing/AssessmentResult_Correct.xml")));
            assessmentResult.ChangeItemResult("ITM-SN02945", "0");
            assessmentResult.ChangeItemResult("ITM-SN02946", "0");
            // act
            OutcomeProcessor.Process(assessmentTest, assessmentResult, logger);
            //assert
            Assert.Equal("0", assessmentResult.GetScoreForTest("TST-Test_toets", "SCORE"));
        }

        [Fact]
        public void Outcome_All_Incorrect()
        {
            //arrange
            var mockLogger = new Mock<ILogger>();
            var logger = mockLogger.Object;
            var assessmentTest = new AssessmentTest(logger, XDocument.Load(File.OpenRead($"Resources/2x/OutcomeProcessing/Test_Toets2.xml")), TestHelper.GetExpressionFactory());
            var assessmentResult = new AssessmentResult(logger, XDocument.Load(File.OpenRead($"Resources/2x/OutcomeProcessing/AssessmentResult_Correct.xml")));
            assessmentResult.ChangeItemResult("ITM-SN02945", "0");
            assessmentResult.ChangeItemResult("ITM-SN02946", "0");
            // act
            OutcomeProcessor.Process(assessmentTest, assessmentResult, logger);
            //assert
            Assert.Equal("0", assessmentResult.GetScoreForTest("TST-Test_toets", "SCORE"));
        }

        [Fact]
        public void Outcome_All_CorrectAndInCorrect()
        {
            //arrange
            var mockLogger = new Mock<ILogger>();
            var logger = mockLogger.Object;
            var assessmentTest = new AssessmentTest(logger, XDocument.Load(File.OpenRead($"Resources/2x/OutcomeProcessing/Test_Toets2.xml")), TestHelper.GetExpressionFactory());
            var assessmentResult = new AssessmentResult(logger, XDocument.Load(File.OpenRead($"Resources/2x/OutcomeProcessing/AssessmentResult_Correct.xml")));
            assessmentResult.ChangeItemResult("ITM-SN02945", "0");
            assessmentResult.ChangeItemResult("ITM-SN02946", "1");

            // act
            OutcomeProcessor.Process(assessmentTest, assessmentResult, logger);
            //assert
            Assert.Equal("1", assessmentResult.GetScoreForTest("TST-Test_toets", "SCORE"));
        }

        [Fact]
        public void Outcome_All_CorrectAndInCorrect_Weight()
        {
            //arrange
            var mockLogger = new Mock<ILogger>();
            var logger = mockLogger.Object;
            var assessmentTest = new AssessmentTest(logger, XDocument.Load(File.OpenRead($"Resources/2x/OutcomeProcessing/Test_Toets2-Weight.xml")), TestHelper.GetExpressionFactory());
            var assessmentResult = new AssessmentResult(logger, XDocument.Load(File.OpenRead($"Resources/2x/OutcomeProcessing/AssessmentResult_Correct.xml")));
            assessmentResult.ChangeItemResult("ITM-SN02945", "0");
            assessmentResult.ChangeItemResult("ITM-SN02946", "1");

            // act
            OutcomeProcessor.Process(assessmentTest, assessmentResult, logger);
            //assert
            Assert.Equal("0", assessmentResult.GetScoreForTest("TST-Test_toets", "SCORE"));
        }
    }


}
