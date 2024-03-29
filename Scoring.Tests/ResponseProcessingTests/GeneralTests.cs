using System.IO;
using System.Linq;
using System.Xml;
using System.Xml.Linq;
using Citolab.QTI.ScoringEngine.Helpers;
using Citolab.QTI.ScoringEngine.Interfaces;
using Citolab.QTI.ScoringEngine.Model;
using Citolab.QTI.ScoringEngine.ResponseProcessing;
using Microsoft.Extensions.Logging;
using Moq;
using Xunit;
using ILogger = Microsoft.Extensions.Logging.ILogger;

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

            var assessmentResult = new AssessmentResult(logger, XDocument.Load(File.OpenRead("Resources/2x/ResponseProcessing/AssessmentResult_Update_OutcomeVariable.xml")));
            var assessmentItem = new AssessmentItem(logger, XDocument.Load(File.OpenRead("Resources/2x/ResponseProcessing/ITM-50066.xml")), TestHelper.GetExpressionFactory());

            ResponseProcessor.Process(assessmentItem, assessmentResult, logger);

            var scoreValue = assessmentResult.GetScoreForItem("ITM-50066", "SCORE");
            Assert.Equal("1", scoreValue);
        }

        /// <summary>
        /// BUG: https://github.com/Citolab/qti-scoring-engine/issues/1
        /// </summary>
        [Fact]
        public void Rescore_Update_No_Double_Outcomes()
        {
            var logger = new Mock<ILogger>().Object;

            var assessmentResult = new AssessmentResult(logger, XDocument.Load(File.OpenRead("Resources/2x/ResponseProcessing/AssessmentResult_Update_OutcomeVariable.xml")));
            var assessmentItem = new AssessmentItem(logger, XDocument.Load(File.OpenRead("Resources/2x/ResponseProcessing/ITM-50066.xml")), TestHelper.GetExpressionFactory());

            ResponseProcessor.Process(assessmentItem, assessmentResult, logger);
            var itemResult = assessmentResult.FindElementsByElementAndAttributeValue("itemResult", "identifier", "ITM-50066").FirstOrDefault();
            var numberOfOutcomeScoreTotalElements = itemResult.FindElementsByElementAndAttributeValue("outcomeVariable", "identifier", "SCORE").Count();

            Assert.Equal(1, numberOfOutcomeScoreTotalElements);
        }

        [Fact]
        public void Rescore_Update_OutcomeVariable_Incorrect()
        {
            var logger = new Mock<ILogger>().Object;

            var assessmentResult = new AssessmentResult(logger, XDocument.Load(File.OpenRead("Resources/2x/ResponseProcessing/AssessmentResult_Update_OutcomeVariable.xml")));
            var assessmentItem = new AssessmentItem(logger, XDocument.Load(File.OpenRead("Resources/2x/ResponseProcessing/ITM-50066.xml")), TestHelper.GetExpressionFactory());

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

            var assessmentResult = new AssessmentResult(logger, XDocument.Load(File.OpenRead("Resources/2x/ResponseProcessing/AssessmentResult_Add_OutcomeVariable.xml")));
            var assessmentItem = new AssessmentItem(logger, XDocument.Load(File.OpenRead("Resources/2x/ResponseProcessing/ITM-50066.xml")), TestHelper.GetExpressionFactory());

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

            var assessmentResult = new AssessmentResult(logger, XDocument.Load(File.OpenRead("Resources/2x/ResponseProcessing/AssessmentResult_Add_OutcomeVariable.xml")));
            var assessmentItem = new AssessmentItem(logger, XDocument.Load(File.OpenRead("Resources/2x/ResponseProcessing/ITM-50066_No_Correct_Response.xml")), TestHelper.GetExpressionFactory());

            ResponseProcessor.Process(assessmentItem, assessmentResult, logger);

            var scoreValue = assessmentResult.GetScoreForItem("ITM-50066", "SCORE");
            Assert.Equal("1", scoreValue);
        }

        [Fact]
        public void NoResponseDeclaringDontChangeManualScore()
        {
            var logger = new Mock<ILogger>().Object;
            var assessmentResult = new AssessmentResult(logger, XDocument.Load(File.OpenRead("Resources/2x/OutcomeProcessing/AssessmentResult_Manual_Scored.xml")));
            var assessmentItem = new AssessmentItem(logger, XDocument.Load(File.OpenRead("Resources/2x/ResponseProcessing/IMS-examples/extended_text.xml")), TestHelper.GetExpressionFactory());

            ResponseProcessor.Process(assessmentItem, assessmentResult, logger);
            var scoreValue = assessmentResult.GetScoreForItem("extendedTextEntry", "SCORE");
            Assert.Equal("2", scoreValue);
        }

        /// <summary>
        /// Verify that the score is equal to zero and everything work without a candidateResponse node
        /// </summary>
        [Fact]
        public void No_Candidate_Response()
        {
            var logger = new Mock<ILogger>().Object;

            var assessmentResult = new AssessmentResult(logger, XDocument.Load(File.OpenRead("Resources/2x/ResponseProcessing/AssessmentResult_No_Candidate_Response.xml")));
            var assessmentItem = new AssessmentItem(logger, XDocument.Load(File.OpenRead("Resources/2x/ResponseProcessing/ITM-50069.xml")), TestHelper.GetExpressionFactory());

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

            var assessmentResult = new AssessmentResult(mockLogger.Object, XDocument.Load(File.OpenRead("Resources/2x/ResponseProcessing/AssessmentResult_Interpolation.xml")));
            var assessmentItem = new AssessmentItem(mockLogger.Object, XDocument.Load(File.OpenRead("Resources/2x/ResponseProcessing/083-Verklanking-Speciale-Tekens_No_Identifier_For_Variable_In_LookupOutcomeValue.xml")), TestHelper.GetExpressionFactory());

            ResponseProcessor.Process(assessmentItem, assessmentResult, mockLogger.Object);
            //No outcome identifier could be found for the raw score to use with interpolation.
            mockLogger.VerifyLog((state, t) => state.ContainsValue("No outcome identifier could be found for the raw score to use with interpolation."), LogLevel.Error, 1);
        }

        [Fact]
        public void ResponseProcessing_30_ExternalMachine()
        {
            var mockLogger = new Mock<ILogger>();

            var assessmentResult = new AssessmentResult(mockLogger.Object, XDocument.Load(File.OpenRead("Resources/30/ResponseProcessing/IMS-examples/assessment_result_external_machine.xml")));
            var assessmentItem = new AssessmentItem(mockLogger.Object, XDocument.Load(File.OpenRead("Resources/30/ResponseProcessing/IMS-examples/text-entry-qti3.xml")), TestHelper.GetExpressionFactory());

            ResponseProcessor.Process(assessmentItem, assessmentResult, mockLogger.Object);

            var scoreValue = assessmentResult.GetScoreForItem("textEntry", "SCORE");
            Assert.Equal("0.5", scoreValue);
        }

        [Fact]
        public void ResponseProcessing_30_Human()
        {
            var mockLogger = new Mock<ILogger>();

            var assessmentResult = new AssessmentResult(mockLogger.Object, XDocument.Load(File.OpenRead("Resources/30/ResponseProcessing/IMS-examples/assessment_result_human.xml")));
            var assessmentItem = new AssessmentItem(mockLogger.Object, XDocument.Load(File.OpenRead("Resources/30/ResponseProcessing/IMS-examples/text-entry-qti3.xml")), TestHelper.GetExpressionFactory());

            ResponseProcessor.Process(assessmentItem, assessmentResult, mockLogger.Object);

            var scoreValue = assessmentResult.GetScoreForItem("textEntry", "SCORE");
            // variable should be untouched when human scored
            Assert.Equal("1", scoreValue);
        }

        [Fact]
        public void ResponseProcessing_DST_Human()
        {
            var mockLogger = new Mock<ILogger>();

            var assessmentResult = new AssessmentResult(mockLogger.Object, XDocument.Load(File.OpenRead("Resources/2x/ResponseProcessing/AssessmentResult_DST_pap.xml")));
            var assessmentItem = new AssessmentItem(mockLogger.Object, XDocument.Load(File.OpenRead("Resources/2x/ResponseProcessing/ITM-equal.xml")), TestHelper.GetExpressionFactory());

            ResponseProcessor.Process(assessmentItem, assessmentResult, mockLogger.Object);

            var scoreValue = assessmentResult.GetScoreForItem("ITM-equal", "SCORE");
            // variable should be untouched when human scored
            Assert.Equal("1", scoreValue);
        }

        [Fact]
        public void ResponseProcessing_DST_Raw_score()
        {
            var mockLogger = new Mock<ILogger>();

            var assessmentResult = new AssessmentResult(mockLogger.Object, XDocument.Load(File.OpenRead("Resources/30/ResponseProcessing/assessmentresult_DOE.xml")));
            var assessmentItem = new AssessmentItem(mockLogger.Object, XDocument.Load(File.OpenRead("Resources/30/ResponseProcessing/d3fc4cde-5dc0-451c-b2a5-3df3ba554e3e.xml")), TestHelper.GetExpressionFactory());

            ResponseProcessor.Process(assessmentItem, assessmentResult, mockLogger.Object);


            var scoreValue = assessmentResult.GetScoreForItem("_d3fc4cde-5dc0-451c-b2a5-3df3ba554e3e", "SCORE");
            var rawScore = assessmentResult.GetScoreForItem("_d3fc4cde-5dc0-451c-b2a5-3df3ba554e3e", "RAWSCORE");
            // variable should be untouched when human scored
            Assert.Equal("0", scoreValue);
            Assert.Equal("0", rawScore);
        }

        [Fact]
        public void ResponseProcessing_DOE_gapMatch()
        {
            var mockLogger = new Mock<ILogger>();

            var assessmentResult = new AssessmentResult(mockLogger.Object, XDocument.Load(File.OpenRead("Resources/30/ResponseProcessing/gap-match-result.xml")));
            var assessmentItem = new AssessmentItem(mockLogger.Object, XDocument.Load(File.OpenRead("Resources/30/ResponseProcessing/gap-match-item.xml")), TestHelper.GetExpressionFactory());

            ResponseProcessor.Process(assessmentItem, assessmentResult, mockLogger.Object);

            var scoreValue = assessmentResult.GetScoreForItem("_gap_match", "SCORE");
            var rawScore = assessmentResult.GetScoreForItem("_gap_match", "RAWSCORE");
            // variable should be untouched when human scored
            Assert.Equal("1", scoreValue);
            Assert.Equal("1", rawScore);
        }

        [Fact]
        public void ResponseProcessing_DOE_hotspot_correct_1()
        {
            var mockLogger = new Mock<ILogger>();

            var assessmentResult = new AssessmentResult(mockLogger.Object, XDocument.Load(File.OpenRead("Resources/30/ResponseProcessing/hs-result-correct-1.xml")));
            var assessmentItem = new AssessmentItem(mockLogger.Object, XDocument.Load(File.OpenRead("Resources/30/ResponseProcessing/hs.xml")), TestHelper.GetExpressionFactory());

            ResponseProcessor.Process(assessmentItem, assessmentResult, mockLogger.Object);

            var scoreValue = assessmentResult.GetScoreForItem("_111", "SCORE");
            Assert.Equal("1", scoreValue);
        }

        [Fact]
        public void ResponseProcessing_DOE_workaround_hotspot_correct_1()
        {
            var mockLogger = new Mock<ILogger>();

            var assessmentResult = new AssessmentResult(mockLogger.Object, XDocument.Load(File.OpenRead("Resources/30/ResponseProcessing/hs-result-correct-1.xml")));
            var assessmentItem = new AssessmentItem(mockLogger.Object, XDocument.Load(File.OpenRead("Resources/30/ResponseProcessing/hs-workaround.xml")), TestHelper.GetExpressionFactory());

            ResponseProcessor.Process(assessmentItem, assessmentResult, mockLogger.Object);

            var scoreValue = assessmentResult.GetScoreForItem("_111", "SCORE");
            Assert.Equal("1", scoreValue);
        }

        [Fact]
        public void ResponseProcessing_DOE_hotspot_correct_2()
        {
            var mockLogger = new Mock<ILogger>();

            var assessmentResult = new AssessmentResult(mockLogger.Object, XDocument.Load(File.OpenRead("Resources/30/ResponseProcessing/hs-result-correct-2.xml")));
            var assessmentItem = new AssessmentItem(mockLogger.Object, XDocument.Load(File.OpenRead("Resources/30/ResponseProcessing/hs.xml")), TestHelper.GetExpressionFactory());

            ResponseProcessor.Process(assessmentItem, assessmentResult, mockLogger.Object);

            var scoreValue = assessmentResult.GetScoreForItem("_111", "SCORE");
            Assert.Equal("1", scoreValue);
        }

        [Fact]
        public void ResponseProcessing_DOE_workaround_hotspot_correct_2()
        {
            var mockLogger = new Mock<ILogger>();

            var assessmentResult = new AssessmentResult(mockLogger.Object, XDocument.Load(File.OpenRead("Resources/30/ResponseProcessing/hs-result-correct-2.xml")));
            var assessmentItem = new AssessmentItem(mockLogger.Object, XDocument.Load(File.OpenRead("Resources/30/ResponseProcessing/hs-workaround.xml")), TestHelper.GetExpressionFactory());

            ResponseProcessor.Process(assessmentItem, assessmentResult, mockLogger.Object);

            var scoreValue = assessmentResult.GetScoreForItem("_111", "SCORE");
            Assert.Equal("1", scoreValue);
        }


        [Fact]
        public void ResponseProcessing_DOE_hotspot_incorrect_1()
        {
            var mockLogger = new Mock<ILogger>();

            var assessmentResult = new AssessmentResult(mockLogger.Object, XDocument.Load(File.OpenRead("Resources/30/ResponseProcessing/hs-result-incorrect-1.xml")));
            var assessmentItem = new AssessmentItem(mockLogger.Object, XDocument.Load(File.OpenRead("Resources/30/ResponseProcessing/hs.xml")), TestHelper.GetExpressionFactory());

            ResponseProcessor.Process(assessmentItem, assessmentResult, mockLogger.Object);

            var scoreValue = assessmentResult.GetScoreForItem("_111", "SCORE");
            Assert.Equal("0", scoreValue);
        }

        [Fact]
        public void ResponseProcessing_DOE_workaround_hotspot_incorrect_1()
        {
            var mockLogger = new Mock<ILogger>();

            var assessmentResult = new AssessmentResult(mockLogger.Object, XDocument.Load(File.OpenRead("Resources/30/ResponseProcessing/hs-result-incorrect-1.xml")));
            var assessmentItem = new AssessmentItem(mockLogger.Object, XDocument.Load(File.OpenRead("Resources/30/ResponseProcessing/hs-workaround.xml")), TestHelper.GetExpressionFactory());

            ResponseProcessor.Process(assessmentItem, assessmentResult, mockLogger.Object);

            var scoreValue = assessmentResult.GetScoreForItem("_111", "SCORE");
            Assert.Equal("0", scoreValue);
        }

        [Fact]
        public void ResponseProcessing_DOE_hotspot_incorrect_2()
        {
            var mockLogger = new Mock<ILogger>();

            var assessmentResult = new AssessmentResult(mockLogger.Object, XDocument.Load(File.OpenRead("Resources/30/ResponseProcessing/hs-result-incorrect-2.xml")));
            var assessmentItem = new AssessmentItem(mockLogger.Object, XDocument.Load(File.OpenRead("Resources/30/ResponseProcessing/hs.xml")), TestHelper.GetExpressionFactory());

            ResponseProcessor.Process(assessmentItem, assessmentResult, mockLogger.Object);

            var scoreValue = assessmentResult.GetScoreForItem("_111", "SCORE");
            Assert.Equal("0", scoreValue);
        }

        [Fact]
        public void ResponseProcessing_DOE_workaround_hotspot_incorrect_2()
        {
            var mockLogger = new Mock<ILogger>();

            var assessmentResult = new AssessmentResult(mockLogger.Object, XDocument.Load(File.OpenRead("Resources/30/ResponseProcessing/hs-result-incorrect-2.xml")));
            var assessmentItem = new AssessmentItem(mockLogger.Object, XDocument.Load(File.OpenRead("Resources/30/ResponseProcessing/hs-workaround.xml")), TestHelper.GetExpressionFactory());

            ResponseProcessor.Process(assessmentItem, assessmentResult, mockLogger.Object);

            var scoreValue = assessmentResult.GetScoreForItem("_111", "SCORE");
            Assert.Equal("0", scoreValue);
        }

        [Fact]
        public void ResponseProcessing_DOE_2gaps_correct_1()
        {
            var mockLogger = new Mock<ILogger>();

            var assessmentResult = new AssessmentResult(mockLogger.Object, XDocument.Load(File.OpenRead("Resources/30/ResponseProcessing/2gaps-result-correct-1.xml")));
            var assessmentItem = new AssessmentItem(mockLogger.Object, XDocument.Load(File.OpenRead("Resources/30/ResponseProcessing/2gaps.xml")), TestHelper.GetExpressionFactory());

            ResponseProcessor.Process(assessmentItem, assessmentResult, mockLogger.Object);

            var scoreValue = assessmentResult.GetScoreForItem("_2gaps", "SCORE");
            Assert.Equal("1", scoreValue);
        }

        [Fact]
        public void ResponseProcessing_DOE_2gaps_workaround_correct_1()
        {
            var mockLogger = new Mock<ILogger>();

            var assessmentResult = new AssessmentResult(mockLogger.Object, XDocument.Load(File.OpenRead("Resources/30/ResponseProcessing/2gaps-result-correct-1.xml")));
            var assessmentItem = new AssessmentItem(mockLogger.Object, XDocument.Load(File.OpenRead("Resources/30/ResponseProcessing/2gaps-workaround.xml")), TestHelper.GetExpressionFactory());

            ResponseProcessor.Process(assessmentItem, assessmentResult, mockLogger.Object);

            var scoreValue = assessmentResult.GetScoreForItem("_2gaps", "SCORE");
            Assert.Equal("1", scoreValue);
        }

        [Fact]
        public void ResponseProcessing_DOE_2gaps_correct_2()
        {
            var mockLogger = new Mock<ILogger>();

            var assessmentResult = new AssessmentResult(mockLogger.Object, XDocument.Load(File.OpenRead("Resources/30/ResponseProcessing/2gaps-result-correct-2.xml")));
            var assessmentItem = new AssessmentItem(mockLogger.Object, XDocument.Load(File.OpenRead("Resources/30/ResponseProcessing/2gaps.xml")), TestHelper.GetExpressionFactory());

            ResponseProcessor.Process(assessmentItem, assessmentResult, mockLogger.Object);

            var scoreValue = assessmentResult.GetScoreForItem("_2gaps", "SCORE");
            Assert.Equal("1", scoreValue);
        }

        [Fact]
        public void ResponseProcessing_DOE_2gaps_workaround_correct_2()
        {
            var mockLogger = new Mock<ILogger>();

            var assessmentResult = new AssessmentResult(mockLogger.Object, XDocument.Load(File.OpenRead("Resources/30/ResponseProcessing/2gaps-result-correct-2.xml")));
            var assessmentItem = new AssessmentItem(mockLogger.Object, XDocument.Load(File.OpenRead("Resources/30/ResponseProcessing/2gaps-workaround.xml")), TestHelper.GetExpressionFactory());

            ResponseProcessor.Process(assessmentItem, assessmentResult, mockLogger.Object);

            var scoreValue = assessmentResult.GetScoreForItem("_2gaps", "SCORE");
            Assert.Equal("1", scoreValue);
        }

        [Fact]
        public void ResponseProcessing_DOE_2gaps_incorrect_1()
        {
            var mockLogger = new Mock<ILogger>();

            var assessmentResult = new AssessmentResult(mockLogger.Object, XDocument.Load(File.OpenRead("Resources/30/ResponseProcessing/2gaps-result-incorrect-1.xml")));
            var assessmentItem = new AssessmentItem(mockLogger.Object, XDocument.Load(File.OpenRead("Resources/30/ResponseProcessing/2gaps.xml")), TestHelper.GetExpressionFactory());

            ResponseProcessor.Process(assessmentItem, assessmentResult, mockLogger.Object);

            var scoreValue = assessmentResult.GetScoreForItem("_2gaps", "SCORE");
            Assert.Equal("0", scoreValue);
        }

        [Fact]
        public void ResponseProcessing_DOE_2gaps_workaround_incorrect_1()
        {
            var mockLogger = new Mock<ILogger>();

            var assessmentResult = new AssessmentResult(mockLogger.Object, XDocument.Load(File.OpenRead("Resources/30/ResponseProcessing/2gaps-result-incorrect-1.xml")));
            var assessmentItem = new AssessmentItem(mockLogger.Object, XDocument.Load(File.OpenRead("Resources/30/ResponseProcessing/2gaps-workaround.xml")), TestHelper.GetExpressionFactory());

            ResponseProcessor.Process(assessmentItem, assessmentResult, mockLogger.Object);

            var scoreValue = assessmentResult.GetScoreForItem("_2gaps", "SCORE");
            Assert.Equal("0", scoreValue);
        }

        [Fact]
        public void ResponseProcessing_DOE_2gaps_incorrect_2()
        {
            var mockLogger = new Mock<ILogger>();

            var assessmentResult = new AssessmentResult(mockLogger.Object, XDocument.Load(File.OpenRead("Resources/30/ResponseProcessing/2gaps-result-incorrect-2.xml")));
            var assessmentItem = new AssessmentItem(mockLogger.Object, XDocument.Load(File.OpenRead("Resources/30/ResponseProcessing/2gaps.xml")), TestHelper.GetExpressionFactory());

            ResponseProcessor.Process(assessmentItem, assessmentResult, mockLogger.Object);

            var scoreValue = assessmentResult.GetScoreForItem("_2gaps", "SCORE");
            Assert.Equal("0", scoreValue);
        }


        [Fact]
        public void ResponseProcessing_DOE_2gaps_workaround_incorrect_2()
        {
            var mockLogger = new Mock<ILogger>();

            var assessmentResult = new AssessmentResult(mockLogger.Object, XDocument.Load(File.OpenRead("Resources/30/ResponseProcessing/2gaps-result-incorrect-2.xml")));
            var assessmentItem = new AssessmentItem(mockLogger.Object, XDocument.Load(File.OpenRead("Resources/30/ResponseProcessing/2gaps-workaround.xml")), TestHelper.GetExpressionFactory());

            ResponseProcessor.Process(assessmentItem, assessmentResult, mockLogger.Object);

            var scoreValue = assessmentResult.GetScoreForItem("_2gaps", "SCORE");
            Assert.Equal("0", scoreValue);
        }


        [Fact]
        public void ResponseProcessing_DOE_2gaps_incorrect_3()
        {
            var mockLogger = new Mock<ILogger>();

            var assessmentResult = new AssessmentResult(mockLogger.Object, XDocument.Load(File.OpenRead("Resources/30/ResponseProcessing/2gaps-result-incorrect-3.xml")));
            var assessmentItem = new AssessmentItem(mockLogger.Object, XDocument.Load(File.OpenRead("Resources/30/ResponseProcessing/2gaps.xml")), TestHelper.GetExpressionFactory());

            ResponseProcessor.Process(assessmentItem, assessmentResult, mockLogger.Object);

            var scoreValue = assessmentResult.GetScoreForItem("_2gaps", "SCORE");
            Assert.Equal("0", scoreValue);
        }


        [Fact]
        public void ResponseProcessing_DOE_2gaps_workaround_incorrect_3()
        {
            var mockLogger = new Mock<ILogger>();

            var assessmentResult = new AssessmentResult(mockLogger.Object, XDocument.Load(File.OpenRead("Resources/30/ResponseProcessing/2gaps-result-incorrect-3.xml")));
            var assessmentItem = new AssessmentItem(mockLogger.Object, XDocument.Load(File.OpenRead("Resources/30/ResponseProcessing/2gaps-workaround.xml")), TestHelper.GetExpressionFactory());

            ResponseProcessor.Process(assessmentItem, assessmentResult, mockLogger.Object);

            var scoreValue = assessmentResult.GetScoreForItem("_2gaps", "SCORE");
            Assert.Equal("0", scoreValue);
        }

        [Fact]
        public void ResponseProcessing_DOE_gapMatch_fout()
        {
            var mockLogger = new Mock<ILogger>();

            var assessmentResult = new AssessmentResult(mockLogger.Object, XDocument.Load(File.OpenRead("Resources/30/ResponseProcessing/gap-match-result-fout.xml")));
            var assessmentItem = new AssessmentItem(mockLogger.Object, XDocument.Load(File.OpenRead("Resources/30/ResponseProcessing/gap-match-item.xml")), TestHelper.GetExpressionFactory());

            ResponseProcessor.Process(assessmentItem, assessmentResult, mockLogger.Object);

            var scoreValue = assessmentResult.GetScoreForItem("_gap_match", "SCORE");
            var rawScore = assessmentResult.GetScoreForItem("_gap_match", "RAWSCORE");
            // variable should be untouched when human scored
            Assert.Equal("0", scoreValue);
            Assert.Equal("0", rawScore);
        }

        [Fact]
        public void ResponseProcessing_DOE_sum_all_correct()
        {
            var mockLogger = new Mock<ILogger>();

            var assessmentResult = new AssessmentResult(mockLogger.Object, XDocument.Load(File.OpenRead("Resources/30/ResponseProcessing/sum_result.xml")));
            var assessmentItem = new AssessmentItem(mockLogger.Object, XDocument.Load(File.OpenRead("Resources/30/ResponseProcessing/sum_item.xml")), TestHelper.GetExpressionFactory());

            ResponseProcessor.Process(assessmentItem, assessmentResult, mockLogger.Object);

            var scoreValue = assessmentResult.GetScoreForItem("_sum", "SCORE");
            Assert.Equal("1", scoreValue);
        }
        [Fact]
        public void ResponseProcessing_DOE_sum_3_correct()
        {
            var mockLogger = new Mock<ILogger>();

            var assessmentResult = new AssessmentResult(mockLogger.Object, XDocument.Load(File.OpenRead("Resources/30/ResponseProcessing/sum_result.xml")));
            var assessmentItem = new AssessmentItem(mockLogger.Object, XDocument.Load(File.OpenRead("Resources/30/ResponseProcessing/sum_item.xml")), TestHelper.GetExpressionFactory());

            assessmentResult.ChangeResponse("_sum", "RESPONSE_1", "fout");

            ResponseProcessor.Process(assessmentItem, assessmentResult, mockLogger.Object);

            var scoreValue = assessmentResult.GetScoreForItem("_sum", "SCORE");
            Assert.Equal("0", scoreValue);
        }
        [Fact]
        public void ResponseProcessing_DOE_sum_2_correct_different_result_file()
        {
            var mockLogger = new Mock<ILogger>();

            var assessmentResult = new AssessmentResult(mockLogger.Object, XDocument.Load(File.OpenRead("Resources/30/ResponseProcessing/sum_result_2.xml")));
            var assessmentItem = new AssessmentItem(mockLogger.Object, XDocument.Load(File.OpenRead("Resources/30/ResponseProcessing/sum_item.xml")), TestHelper.GetExpressionFactory());

            ResponseProcessor.Process(assessmentItem, assessmentResult, mockLogger.Object);

            var scoreValue = assessmentResult.GetScoreForItem("_sum", "SCORE");
            Assert.Equal("0", scoreValue);
        }
        [Fact]
        public void ResponseProcessing_DOE_sum_0_correct()
        {
            var mockLogger = new Mock<ILogger>();

            var assessmentResult = new AssessmentResult(mockLogger.Object, XDocument.Load(File.OpenRead("Resources/30/ResponseProcessing/sum_result.xml")));
            var assessmentItem = new AssessmentItem(mockLogger.Object, XDocument.Load(File.OpenRead("Resources/30/ResponseProcessing/sum_item.xml")), TestHelper.GetExpressionFactory());
            assessmentResult.ChangeResponse("_sum", "RESPONSE_1", "fout");
            assessmentResult.ChangeResponse("_sum", "RESPONSE_2", "fout");
            assessmentResult.ChangeResponse("_sum", "RESPONSE_3", "fout");
            assessmentResult.ChangeResponse("_sum", "RESPONSE_4", "fout");
            ResponseProcessor.Process(assessmentItem, assessmentResult, mockLogger.Object);

            var scoreValue = assessmentResult.GetScoreForItem("_sum", "SCORE");
            Assert.Equal("0", scoreValue);
        }

        [Fact]
        public void ResponseProcessing_DOE_numeric_correct()
        {
            var mockLogger = new Mock<ILogger>();

            var assessmentResult = new AssessmentResult(mockLogger.Object, XDocument.Load(File.OpenRead("Resources/30/ResponseProcessing/result_numeric.xml")));
            var assessmentItem = new AssessmentItem(mockLogger.Object, XDocument.Load(File.OpenRead("Resources/30/ResponseProcessing/e726a093-a5dd-456e-8cc4-fd987cd05438.xml")), TestHelper.GetExpressionFactory());

            ResponseProcessor.Process(assessmentItem, assessmentResult, mockLogger.Object);

            var textScore = assessmentResult.GetScoreForItem("_e726a093-a5dd-456e-8cc4-fd987cd05438", "RAWSCORE_RESPONSE1");
            var intScore = assessmentResult.GetScoreForItem("_e726a093-a5dd-456e-8cc4-fd987cd05438", "RAWSCORE_RESPONSE2");
            var floatScore = assessmentResult.GetScoreForItem("_e726a093-a5dd-456e-8cc4-fd987cd05438", "RAWSCORE_RESPONSE3");

            Assert.Equal("1", textScore);
            Assert.Equal("1", intScore);
            Assert.Equal("1", floatScore);
        }

        [Fact]
        public void ResponseProcessing_DOE_numeric_incorrect()
        {
            var mockLogger = new Mock<ILogger>();

            var assessmentResult = new AssessmentResult(mockLogger.Object, XDocument.Load(File.OpenRead("Resources/30/ResponseProcessing/result_numeric.xml")));
            var assessmentItem = new AssessmentItem(mockLogger.Object, XDocument.Load(File.OpenRead("Resources/30/ResponseProcessing/e726a093-a5dd-456e-8cc4-fd987cd05438.xml")), TestHelper.GetExpressionFactory());

            assessmentResult.ChangeResponse("_e726a093-a5dd-456e-8cc4-fd987cd05438", "RESPONSE_1", "fout!");
            assessmentResult.ChangeResponse("_e726a093-a5dd-456e-8cc4-fd987cd05438", "RESPONSE_2", "100cm");
            assessmentResult.ChangeResponse("_e726a093-a5dd-456e-8cc4-fd987cd05438", "RESPONSE_3", "2,14 denk ik");

            ResponseProcessor.Process(assessmentItem, assessmentResult, mockLogger.Object);

            var textScore = assessmentResult.GetScoreForItem("_e726a093-a5dd-456e-8cc4-fd987cd05438", "RAWSCORE_RESPONSE1");
            var intScore = assessmentResult.GetScoreForItem("_e726a093-a5dd-456e-8cc4-fd987cd05438", "RAWSCORE_RESPONSE2");
            var floatScore = assessmentResult.GetScoreForItem("_e726a093-a5dd-456e-8cc4-fd987cd05438", "RAWSCORE_RESPONSE3");

            Assert.Equal("0", textScore);
            Assert.Equal("0", intScore);
            Assert.Equal("0", floatScore);
        }

        [Fact]
        public void ResponseProcessing_DOE_numeric_as_string()
        {
            var mockLogger = new Mock<ILogger>();

            var assessmentResult = new AssessmentResult(mockLogger.Object, XDocument.Load(File.OpenRead("Resources/30/ResponseProcessing/result_numeric.xml")));
            var assessmentItem = new AssessmentItem(mockLogger.Object, XDocument.Load(File.OpenRead("Resources/30/ResponseProcessing/e726a093-a5dd-456e-8cc4-fd987cd05438.xml")), TestHelper.GetExpressionFactory());


            assessmentResult.ChangeResponse("_e726a093-a5dd-456e-8cc4-fd987cd05438", "RESPONSE_3", "03,14");

            ResponseProcessor.Process(assessmentItem, assessmentResult, mockLogger.Object);

            var floatScore = assessmentResult.GetScoreForItem("_e726a093-a5dd-456e-8cc4-fd987cd05438", "RAWSCORE_RESPONSE3");

            Assert.Equal("1", floatScore);
        }

        [Fact]
        public void ResponseProcessing_DOE_numeric_without_removeAplhaNumerics()
        {
            var mockLogger = new Mock<ILogger>();

            var assessmentResult = new AssessmentResult(mockLogger.Object, XDocument.Load(File.OpenRead("Resources/30/ResponseProcessing/result_numeric.xml")));
            var assessmentItem = new AssessmentItem(mockLogger.Object, XDocument.Load(File.OpenRead("Resources/30/ResponseProcessing/e726a093-a5dd-456e-8cc4-fd987cd05438.xml")), TestHelper.GetExpressionFactory());

            assessmentResult.ChangeResponse("_e726a093-a5dd-456e-8cc4-fd987cd05438", "RESPONSE_2", "100cm");
            assessmentResult.ChangeResponse("_e726a093-a5dd-456e-8cc4-fd987cd05438", "RESPONSE_3", "3,14 denk ik");

            ResponseProcessor.Process(assessmentItem, assessmentResult, mockLogger.Object);

            var intScore = assessmentResult.GetScoreForItem("_e726a093-a5dd-456e-8cc4-fd987cd05438", "RAWSCORE_RESPONSE2");
            var floatScore = assessmentResult.GetScoreForItem("_e726a093-a5dd-456e-8cc4-fd987cd05438", "RAWSCORE_RESPONSE3");

            Assert.Equal("0", intScore);
            Assert.Equal("0", floatScore);
        }

        [Fact]
        public void ResponseProcessing_DOE_numeric_with_removeAplhaNumerics()
        {
            var mockLogger = new Mock<ILogger>();

            var assessmentResult = new AssessmentResult(mockLogger.Object, XDocument.Load(File.OpenRead("Resources/30/ResponseProcessing/result_numeric.xml")));
            var assessmentItem = new AssessmentItem(mockLogger.Object, XDocument.Load(File.OpenRead("Resources/30/ResponseProcessing/e726a093-a5dd-456e-8cc4-fd987cd05438.xml")), TestHelper.GetExpressionFactory());

            assessmentResult.ChangeResponse("_e726a093-a5dd-456e-8cc4-fd987cd05438", "RESPONSE_2", "100cm");
            assessmentResult.ChangeResponse("_e726a093-a5dd-456e-8cc4-fd987cd05438", "RESPONSE_3", "3,14 denk ik");
            var options = new ResponseProcessingScoringsOptions
            {
                StripAlphanumericsFromNumericResponses = true
            };
            ResponseProcessor.Process(assessmentItem, assessmentResult, mockLogger.Object, options);

            var intScore = assessmentResult.GetScoreForItem("_e726a093-a5dd-456e-8cc4-fd987cd05438", "RAWSCORE_RESPONSE2");
            var floatScore = assessmentResult.GetScoreForItem("_e726a093-a5dd-456e-8cc4-fd987cd05438", "RAWSCORE_RESPONSE3");

            Assert.Equal("1", intScore);
            Assert.Equal("1", floatScore);
        }

        [Fact]
        public void ResponseProcessing_DST_Raw_score_correct()
        {
            var mockLogger = new Mock<ILogger>();

            var assessmentResult = new AssessmentResult(mockLogger.Object, XDocument.Load(File.OpenRead("Resources/30/ResponseProcessing/assessmentresult_DOE.xml")));
            var assessmentItem = new AssessmentItem(mockLogger.Object, XDocument.Load(File.OpenRead("Resources/30/ResponseProcessing/d3fc4cde-5dc0-451c-b2a5-3df3ba554e3e.xml")), TestHelper.GetExpressionFactory());

            assessmentResult.ChangeResponse("_d3fc4cde-5dc0-451c-b2a5-3df3ba554e3e", "RESPONSE", "_a65fab45-67d1-4bbd-92e0-47a8eef2d72b");
            ResponseProcessor.Process(assessmentItem, assessmentResult, mockLogger.Object);


            var scoreValue = assessmentResult.GetScoreForItem("_d3fc4cde-5dc0-451c-b2a5-3df3ba554e3e", "SCORE");
            var rawScore = assessmentResult.GetScoreForItem("_d3fc4cde-5dc0-451c-b2a5-3df3ba554e3e", "RAWSCORE");
            // variable should be untouched when human scored
            Assert.Equal("1", scoreValue);
            Assert.Equal("1", rawScore);
        }




        [Theory]
        [InlineData("Resources/30/ResponseProcessing/IMS-examples/assessment_result_external_machine.xml", "Resources/30/ResponseProcessing/IMS-examples/text-entry-qti3.xml")]
        [InlineData("Resources/2x/ResponseProcessing/AssessmentResult_Update_OutcomeVariable.xml", "Resources/2x/ResponseProcessing/ITM-50066.xml")]
        public void ResponseProcessing_Should_Keep_Local_Namespace(string assessmentResultFile, string assessmentItemFile)
        {
            var logger = new Mock<ILogger>().Object;

            var assessmentResult = new AssessmentResult(logger, XDocument.Load(File.OpenRead(assessmentResultFile)));
            var assessmentItem = new AssessmentItem(logger, XDocument.Load(File.OpenRead(assessmentItemFile)), TestHelper.GetExpressionFactory());

            var result = ResponseProcessor.Process(assessmentItem, assessmentResult, logger);

            Assert.NotNull(result);

            // Empty xmlns check can only be done using a raw xml read on an XmlDocument (XDocument filters them out)
            var rawXml = result.ToString();
            var xmlDoc = new XmlDocument();
            xmlDoc.LoadXml(rawXml);
            string xpathQuery = "//*[namespace-uri() = '' and local-name() != '']";

            // No empty xmlns should be found
            XmlNodeList nodesWithEmptyXmlns = xmlDoc.SelectNodes(xpathQuery);
            Assert.Empty(nodesWithEmptyXmlns);
        }
    }
}