using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Xml.Linq;
using Moq;
using Citolab.QTI.ScoringEngine.Interfaces;
using Citolab.QTI.ScoringEngine.Const;
using Citolab.QTI.ScoringEngine.Model;
using Xunit;
using Citolab.QTI.ScoringEngine.ResponseProcessing;
using Microsoft.Extensions.Logging;
using Citolab.QTI.ScoringEngine.Helper;
using System;

namespace Citolab.QTI.ScoringEngine.Tests.Business
{
    public class GeneralTests
    {

        /// <summary>
        /// Verify that the SCORE node is updated
        /// </summary>
        [Fact]
        public void Rescore_Update_OutcomeVariable()
        {
            var logger = new Mock<ILogger>().Object;

            var assessmentResult = new AssessmentResult(logger, XDocument.Load(File.OpenRead("Resources/ResponseProcessing/AssessmentResult_Update_OutcomeVariable.xml")));
            var assessmentItem = new AssessmentItem(logger, XDocument.Load(File.OpenRead("Resources/ResponseProcessing/ITM-50066.xml")));

            
            ResponseProcessor.Process(assessmentItem, assessmentResult, logger);

            var scoreValue = assessmentResult.GetScoreForItem("ITM-50066", "SCORE");
            Assert.Equal("1", scoreValue);
        }

        [Fact]
        public void Rescore_Update_OutcomeVariable_Incorrect()
        {
            var logger = new Mock<ILogger>().Object;

            var assessmentResult = new AssessmentResult(logger, XDocument.Load(File.OpenRead("Resources/ResponseProcessing/AssessmentResult_Update_OutcomeVariable.xml")));
            var assessmentItem = new AssessmentItem(logger, XDocument.Load(File.OpenRead("Resources/ResponseProcessing/ITM-50066.xml")));

            // make response incorrect
            assessmentResult.ChangeResponse("ITM-50066", "RESPONSE", "A");

            
            ResponseProcessor.Process(assessmentItem, assessmentResult, logger);

            var scoreValue = assessmentResult.GetScoreForItem("ITM-50066", "SCORE");
            Assert.Equal("0", scoreValue);
        }

        /// <summary>
        /// Verify that the SCORE node has been added to the assessmentresult
        /// </summary>
        [Fact]
        public void Add_OutcomeVariable_Not_In_AssessmentResult()
        {
            var logger = new Mock<ILogger>().Object;

            var assessmentResult = new AssessmentResult(logger, XDocument.Load(File.OpenRead("Resources/ResponseProcessing/AssessmentResult_Add_OutcomeVariable.xml")));
            var assessmentItem = new AssessmentItem(logger, XDocument.Load(File.OpenRead("Resources/ResponseProcessing/ITM-50066.xml")));

            
            ResponseProcessor.Process(assessmentItem, assessmentResult, logger);

            var scoreValue = assessmentResult.GetScoreForItem("ITM-50066", "SCORE");

            Assert.Equal("1", scoreValue);
        }

        /// <summary>
        /// Verify that the score will be zero when the given response doesn't match the correct answer
        /// </summary>
       

        /// <summary>
        /// MH: this used to throw an exception, but now response processing is applied it succeeds
        /// </summary>
        [Fact]
        public void ResponseProcessing_No_Correct_Response()
        {
            var logger = new Mock<ILogger>().Object;

            var assessmentResult = new AssessmentResult(logger, XDocument.Load(File.OpenRead("Resources/ResponseProcessing/AssessmentResult_Add_OutcomeVariable.xml")));
            var assessmentItem = new AssessmentItem(logger, XDocument.Load(File.OpenRead("Resources/ResponseProcessing/ITM-50066_No_Correct_Response.xml")));

            
            ResponseProcessor.Process(assessmentItem, assessmentResult, logger);

            var scoreValue = assessmentResult.GetScoreForItem("ITM-50066", "SCORE");
            Assert.Equal("1", scoreValue);
        }

           

        /// <summary>
        /// Verify that the score is equal to zero and everything work without a candidateResponse node
        /// </summary>
        [Fact]
        public void No_Candidate_Response()
        {
            var logger = new Mock<ILogger>().Object;

            var assessmentResult = new AssessmentResult(logger, XDocument.Load(File.OpenRead("Resources/ResponseProcessing/AssessmentResult_No_Candidate_Response.xml")));
            var assessmentItem = new AssessmentItem(logger, XDocument.Load(File.OpenRead("Resources/ResponseProcessing/ITM-50069.xml")));

            
            ResponseProcessor.Process(assessmentItem, assessmentResult, logger);

            var scoreValue = assessmentResult.GetScoreForItem("ITM-50069", "SCORE");
            Assert.Equal("0", scoreValue);
        }

      


        /// <summary>
        /// Verify that an exception is thrown when an item has interpolation but no identifier/variable can be found in the LookupOutcomeValue element
        /// </summary>
        /// <returns></returns>
        [Fact]
        public void ResponseProcessing_Interpolation_LogsError_No_Identifier_In_LookupOutcomeValue()
        {
            var mockLogger = new Mock<ILogger>();

            var assessmentResult = new AssessmentResult(mockLogger.Object, XDocument.Load(File.OpenRead("Resources/ResponseProcessing/AssessmentResult_Interpolation.xml")));
            var assessmentItem = new AssessmentItem(mockLogger.Object, XDocument.Load(File.OpenRead("Resources/ResponseProcessing/083-Verklanking-Speciale-Tekens_No_Identifier_For_Variable_In_LookupOutcomeValue.xml")));

            
            ResponseProcessor.Process(assessmentItem, assessmentResult, mockLogger.Object);
            //No outcome identifier could be found for the raw score to use with interpolation.
            mockLogger.VerifyLog((state, t) => state.ContainsValue("No outcome identifier could be found for the raw score to use with interpolation."), LogLevel.Error, 1);
        }
    }
}
