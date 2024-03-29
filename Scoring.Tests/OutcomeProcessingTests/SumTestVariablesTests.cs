﻿using Microsoft.Extensions.Logging;
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
    public class SumTestVariablesTests
    {
        //Test_Toets_Multiple_Subsections
        [Fact]
        public void Outcome_All_Correct()
        {
            //arrange - act
            var assessmentResult = TestHelper.StartOutcomeProcessing("Test_Toets_TestVariables", "AssessmentResult_Correct");
            //assert
            Assert.Equal("2", assessmentResult.GetScoreForTest("TST-Test_toets", "SCORE"));
            Assert.Equal("1", assessmentResult.GetScoreForTest("TST-Test_toets", "SCORE_CAT_1"));
            Assert.Equal("1", assessmentResult.GetScoreForTest("TST-Test_toets", "SCORE_CAT_2"));
        }



        [Fact]
        public void Outcome_All_Correct_Weight()
        {
            //arrange - act
            var assessmentResult = TestHelper.StartOutcomeProcessing("Test_Toets_TestVariables_Weight", "AssessmentResult_Correct");
            //assert
            Assert.Equal("1", assessmentResult.GetScoreForTest("TST-Test_toets", "SCORE"));
            Assert.Equal("1", assessmentResult.GetScoreForTest("TST-Test_toets", "SCORE_CAT_1"));
            Assert.Equal("0", assessmentResult.GetScoreForTest("TST-Test_toets", "SCORE_CAT_2"));
        }


        [Fact]
        public void Outcome_All_Incorrect_Weight()
        {
            //arrange
            var mockLogger = new Mock<ILogger>();
            var logger = mockLogger.Object;
            var assessmentTest = new AssessmentTest(logger, XDocument.Load(File.OpenRead($"Resources/2x/OutcomeProcessing/Test_Toets_TestVariables_Weight.xml")), TestHelper.GetExpressionFactory());
            var assessmentResult = new AssessmentResult(logger, XDocument.Load(File.OpenRead($"Resources/2x/OutcomeProcessing/AssessmentResult_Correct.xml")));
            assessmentResult.ChangeItemResult("ITM-SN02945", "0");
            assessmentResult.ChangeItemResult("ITM-SN02946", "0");
            
            // actt
            OutcomeProcessor.Process(assessmentTest, assessmentResult, logger);
            //assert
            Assert.Equal("0", assessmentResult.GetScoreForTest("TST-Test_toets", "SCORE"));
            Assert.Equal("0", assessmentResult.GetScoreForTest("TST-Test_toets", "SCORE_CAT_1"));
            Assert.Equal("0", assessmentResult.GetScoreForTest("TST-Test_toets", "SCORE_CAT_2"));
        }

        [Fact]
        public void Outcome_All_Incorrect()
        {
            //arrange
            var mockLogger = new Mock<ILogger>();
            var logger = mockLogger.Object;
            var assessmentTest = new AssessmentTest(logger, XDocument.Load(File.OpenRead($"Resources/2x/OutcomeProcessing/Test_Toets_TestVariables.xml")), TestHelper.GetExpressionFactory());
            var assessmentResult = new AssessmentResult(logger, XDocument.Load(File.OpenRead($"Resources/2x/OutcomeProcessing/AssessmentResult_Correct.xml")));
            assessmentResult.ChangeItemResult("ITM-SN02945", "0");
            assessmentResult.ChangeItemResult("ITM-SN02946", "0");
            
            // actt
            OutcomeProcessor.Process(assessmentTest, assessmentResult, logger);
            //assert
            Assert.Equal("0", assessmentResult.GetScoreForTest("TST-Test_toets", "SCORE"));
            Assert.Equal("0", assessmentResult.GetScoreForTest("TST-Test_toets", "SCORE_CAT_1"));
            Assert.Equal("0", assessmentResult.GetScoreForTest("TST-Test_toets", "SCORE_CAT_2"));
        }

        [Fact]
        public void Outcome_All_CorrectAndInCorrect()
        {
            //arrange
            var mockLogger = new Mock<ILogger>();
            var logger = mockLogger.Object;
            var assessmentTest = new AssessmentTest(logger, XDocument.Load(File.OpenRead($"Resources/2x/OutcomeProcessing/Test_Toets_TestVariables.xml")), TestHelper.GetExpressionFactory());
            var assessmentResult = new AssessmentResult(logger, XDocument.Load(File.OpenRead($"Resources/2x/OutcomeProcessing/AssessmentResult_Correct.xml")));
            assessmentResult.ChangeItemResult("ITM-SN02945", "0");
            assessmentResult.ChangeItemResult("ITM-SN02946", "1");
            
            // actt
            OutcomeProcessor.Process(assessmentTest, assessmentResult, logger);
            //assert
            Assert.Equal("1", assessmentResult.GetScoreForTest("TST-Test_toets", "SCORE"));
            Assert.Equal("0", assessmentResult.GetScoreForTest("TST-Test_toets", "SCORE_CAT_1"));
            Assert.Equal("1", assessmentResult.GetScoreForTest("TST-Test_toets", "SCORE_CAT_2"));
        }

        [Fact]
        public void Outcome_All_CorrectAndInCorrect_Weight()
        {
            //arrange
            var mockLogger = new Mock<ILogger>();
            var logger = mockLogger.Object;
            var assessmentTest = new AssessmentTest(logger, XDocument.Load(File.OpenRead($"Resources/2x/OutcomeProcessing/Test_Toets_TestVariables_Weight.xml")), TestHelper.GetExpressionFactory());
            var assessmentResult = new AssessmentResult(logger, XDocument.Load(File.OpenRead($"Resources/2x/OutcomeProcessing/AssessmentResult_Correct.xml")));
            assessmentResult.ChangeItemResult("ITM-SN02945", "0");
            assessmentResult.ChangeItemResult("ITM-SN02946", "1");
            
            // actt
            OutcomeProcessor.Process(assessmentTest, assessmentResult, logger);
            //assert
            Assert.Equal("0", assessmentResult.GetScoreForTest("TST-Test_toets", "SCORE"));
            Assert.Equal("0", assessmentResult.GetScoreForTest("TST-Test_toets", "SCORE_CAT_1"));
            Assert.Equal("0", assessmentResult.GetScoreForTest("TST-Test_toets", "SCORE_CAT_2"));
        }
    }
}
