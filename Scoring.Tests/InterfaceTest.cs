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
using Citolab.QTI.ScoringEngine;
using Citolab.QTI.ScoringEngine.Helpers;
namespace ScoringEngine.Tests
{
    public class InterfaceTest
    {
        [Fact]
        public void ResponseProcessingTest()
        {
            var logger = new Mock<ILogger>().Object;
            var packageFolder = Path.Combine("Resources\\2x\\QTI-Packages\\RW");
            var assessmentResults = new DirectoryInfo(Path.Combine(packageFolder, "AssessmentResults"))
                           .GetFiles("*.xml")
                           .Select(file => TestHelper.GetDocument(file.FullName))
                           .ToList();
            var assessmentItems = new DirectoryInfo(Path.Combine(packageFolder, "Items"))
                .GetFiles("*.xml")
                .Select(file => TestHelper.GetDocument(file.FullName))
                .ToList();

            var qtiScoringEngine = new Citolab.QTI.ScoringEngine.ScoringEngine();
            var scoredAssessmentResults = qtiScoringEngine.ProcessResponses(new ResponseProcessingContext
            {
                AssessmentItems = assessmentItems,
                AssessmentmentResults = assessmentResults,
                Logger = logger
            });

            var result1 = scoredAssessmentResults[0].GetScoreForItem(assessmentItems[0].Root.Identifier(), "SCORE");
            var result2 = scoredAssessmentResults[0].GetScoreForItem(assessmentItems[1].Root.Identifier(), "SCORE");
            var result3 = scoredAssessmentResults[0].GetScoreForItem(assessmentItems[2].Root.Identifier(), "SCORE");
            var result4 = scoredAssessmentResults[0].GetScoreForItem(assessmentItems[3].Root.Identifier(), "SCORE");
           
            Assert.Equal("1", result1);
            Assert.Equal("1", result2);
            Assert.Equal("1", result3);
            Assert.Equal("1", result4);
        }

        [Fact]
        public void ResponseProcessingTest2Incorrect()
        {
            var logger = new Mock<ILogger>().Object;
            var packageFolder = Path.Combine("Resources\\2x\\QTI-Packages\\RW");
            var assessmentResults = new DirectoryInfo(Path.Combine(packageFolder, "AssessmentResults"))
                           .GetFiles("*.xml")
                           .Select(file => TestHelper.GetDocument(file.FullName))
                           .ToList();
            var assessmentItems = new DirectoryInfo(Path.Combine(packageFolder, "Items"))
                .GetFiles("*.xml")
                .Select(file => TestHelper.GetDocument(file.FullName))
                .ToList();

            var qtiScoringEngine = new Citolab.QTI.ScoringEngine.ScoringEngine();

            // make first response incorrect
            assessmentResults.FirstOrDefault().ChangeResponse(assessmentItems[0].Root.Identifier(), "RESPONSE", "1");
            // remove itemResult from second
            assessmentResults.FirstOrDefault().RemoveItemResult(assessmentItems[1].Root.Identifier());

            var scoredAssessmentResults = qtiScoringEngine.ProcessResponses(new ResponseProcessingContext
            {
                AssessmentItems = assessmentItems,
                AssessmentmentResults = assessmentResults,
                Logger = logger
            });

            var result1 = scoredAssessmentResults[0].GetScoreForItem(assessmentItems[0].Root.Identifier(), "SCORE");
            var result2 = scoredAssessmentResults[0].GetScoreForItem(assessmentItems[1].Root.Identifier(), "SCORE");
            var result3 = scoredAssessmentResults[0].GetScoreForItem(assessmentItems[2].Root.Identifier(), "SCORE");
            var result4 = scoredAssessmentResults[0].GetScoreForItem(assessmentItems[3].Root.Identifier(), "SCORE");

            Assert.Equal("0", result1);
            Assert.Null(result2);
            Assert.Equal("1", result3);
            Assert.Equal("1", result4);
        }

        [Fact]
        public void OutcomeProcessingTest()
        {
            var logger = new Mock<ILogger>().Object;
            var packageFolder = Path.Combine("Resources\\2x\\QTI-Packages\\RW");
            var assessmentResults = new DirectoryInfo(Path.Combine(packageFolder, "AssessmentResults"))
                           .GetFiles("*.xml")
                           .Select(file => TestHelper.GetDocument(file.FullName))
                           .ToList();
            var assessmentItems = new DirectoryInfo(Path.Combine(packageFolder, "Items"))
                .GetFiles("*.xml")
                .Select(file => TestHelper.GetDocument(file.FullName))
                .ToList();
            var assessmentTest = new DirectoryInfo(Path.Combine(packageFolder, "Test"))
                .GetFiles("*.xml")
                .Select(file => TestHelper.GetDocument(file.FullName))
                .FirstOrDefault();


            var qtiScoringEngine = new Citolab.QTI.ScoringEngine.ScoringEngine();
            var scoredAssessmentResultWithoutScoredItems = qtiScoringEngine.ProcessOutcomes(new OutcomeProcessingContext
            {
                AssessmentTest = assessmentTest,
                AssessmentmentResults = assessmentResults,
                Logger = logger
            });

            //var result1 = scoredAssessmentResults[0].GetScoreForItem(assessmentItems[0].Root.Identifier(), "SCORE");
            //var result2 = scoredAssessmentResults[0].GetScoreForItem(assessmentItems[1].Root.Identifier(), "SCORE");
            //var result3 = scoredAssessmentResults[0].GetScoreForItem(assessmentItems[2].Root.Identifier(), "SCORE");
            //var result4 = scoredAssessmentResults[0].GetScoreForItem(assessmentItems[3].Root.Identifier(), "SCORE");

            //Assert.Equal("1", result1);
            //Assert.Equal("1", result2);
            //Assert.Equal("1", result3);
            //Assert.Equal("1", result4);
        }
    }
}
