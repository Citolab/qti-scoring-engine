using Citolab.QTI.ScoringEngine.Helpers;
using Citolab.QTI.ScoringEngine.Model;
using Citolab.QTI.ScoringEngine.ResponseProcessing;
using Microsoft.Extensions.Logging;
using Moq;
using System.Collections.Generic;
using System.Linq;
using System.Xml.Linq;
using Xunit;
using ILogger = Microsoft.Extensions.Logging.ILogger;

namespace Citolab.QTI.ScoringEngine.Tests.ResponseProcessingTests
{
    /// <summary>
    /// Outcomes of multiple and ordered cardinality: written by qti-set-outcome-value, read back
    /// with qti-variable, persisted as one value element per member and read again on a rescore.
    /// </summary>
    public class ContainerOutcomeTests
    {
        private const string ItemIdentifier = "ITM-CONTAINER";
        private const string TestIdentifier = "TST-CONTAINER";

        private static string Item(string declarations, string responseProcessing) =>
            $@"<qti-assessment-item xmlns=""http://www.imsglobal.org/xsd/imsqtiasi_v3p0"" identifier=""{ItemIdentifier}"" title=""test"">
                <qti-response-declaration identifier=""RESPONSE"" cardinality=""single"" base-type=""identifier"">
                    <qti-correct-response><qti-value>A</qti-value></qti-correct-response>
                </qti-response-declaration>
                <qti-outcome-declaration identifier=""SCORE"" cardinality=""single"" base-type=""float"">
                    <qti-default-value><qti-value>0</qti-value></qti-default-value>
                </qti-outcome-declaration>
                {declarations}
                <qti-response-processing>{responseProcessing}</qti-response-processing>
            </qti-assessment-item>";

        private const string FeedbackMultiple = @"<qti-outcome-declaration identifier=""FEEDBACK"" cardinality=""multiple"" base-type=""identifier""/>";

        private static string SetFeedback(string expression) =>
            $@"<qti-set-outcome-value identifier=""FEEDBACK"">{expression}</qti-set-outcome-value>";

        /// <summary>
        /// Adds a value to FEEDBACK when the condition is true, the way an author builds it up.
        /// </summary>
        private static string AppendFeedbackWhen(string condition, string value, string container = "qti-multiple") =>
            $@"<qti-response-condition>
                    <qti-response-if>
                        {condition}
                        {SetFeedback($@"<{container}><qti-variable identifier=""FEEDBACK""/><qti-base-value base-type=""identifier"">{value}</qti-base-value></{container}>")}
                    </qti-response-if>
                </qti-response-condition>";

        private static string ScoreWhen(string condition) =>
            $@"<qti-response-condition>
                    <qti-response-if>
                        {condition}
                        <qti-set-outcome-value identifier=""SCORE""><qti-base-value base-type=""float"">1</qti-base-value></qti-set-outcome-value>
                    </qti-response-if>
                </qti-response-condition>";

        private const string IsCorrect = @"<qti-match><qti-variable identifier=""RESPONSE""/><qti-correct identifier=""RESPONSE""/></qti-match>";
        private const string IsIncorrect = @"<qti-not>" + IsCorrect + "</qti-not>";
        private const string Always = @"<qti-base-value base-type=""boolean"">true</qti-base-value>";

        private static AssessmentResult Process(string itemXml, AssessmentResult assessmentResult)
        {
            var logger = new Mock<ILogger>().Object;
            var assessmentItem = new AssessmentItem(logger, XDocument.Parse(itemXml), TestHelper.GetExpressionFactory());
            return ResponseProcessor.Process(assessmentItem, assessmentResult, logger);
        }

        private static AssessmentResult ResultWithResponse(string response = "A") =>
            TestHelper.GetBasicAssessmentResult()
                .AddCandidateResponse(ItemIdentifier, "RESPONSE", response, BaseType.Identifier, Cardinality.Single);

        private static XDocument ScoreWithEngine(string itemXml, XDocument assessmentResult) =>
            new ScoringEngine().ProcessResponses(new ResponseProcessingContext
            {
                AssessmentItems = new List<XDocument> { XDocument.Parse(itemXml) },
                AssessmentmentResults = new List<XDocument> { assessmentResult },
                Logger = new Mock<ILogger>().Object
            })[0];

        private static XElement OutcomeElement(XDocument result, string resultElementName, string resultIdentifier, string outcomeIdentifier) =>
            result.FindElementsByElementAndAttributeValue(resultElementName, "identifier", resultIdentifier)
                .FirstOrDefault()?
                .FindElementsByElementAndAttributeValue("outcomeVariable", "identifier", outcomeIdentifier)
                .FirstOrDefault();

        private static XElement ItemOutcome(XDocument result, string outcomeIdentifier) =>
            OutcomeElement(result, "itemResult", ItemIdentifier, outcomeIdentifier);

        private static List<string> Values(XElement outcomeVariable) =>
            outcomeVariable.Elements().Where(e => e.Name.LocalName == "value").Select(e => e.Value).ToList();

        [Fact]
        public void Regression_Setting_A_Multiple_Outcome_Does_Not_Throw()
        {
            // 1.4.0 threw a NullReferenceException in SetOutcomeValue: a container has no single Value.
            var itemXml = Item(FeedbackMultiple, SetFeedback(@"<qti-multiple>
                    <qti-base-value base-type=""identifier"">correct</qti-base-value>
                </qti-multiple>") + AppendFeedbackWhen(Always, "well_done"));

            var scored = ScoreWithEngine(itemXml, ResultWithResponse());

            var feedback = ItemOutcome(scored, "FEEDBACK");
            Assert.Equal("multiple", feedback.GetAttributeValue("cardinality"));
            Assert.Equal("identifier", feedback.GetAttributeValue("baseType"));
            Assert.Equal(new List<string> { "correct", "well_done" }, Values(feedback));
        }

        [Fact]
        public void A_Multiple_Outcome_Is_Built_From_Scratch()
        {
            var itemXml = Item(FeedbackMultiple, SetFeedback(@"<qti-multiple>
                    <qti-base-value base-type=""identifier"">correct</qti-base-value>
                    <qti-base-value base-type=""identifier"">first_try</qti-base-value>
                </qti-multiple>"));

            var result = Process(itemXml, ResultWithResponse());

            Assert.Equal(new List<string> { "correct", "first_try" }, Values(ItemOutcome(result, "FEEDBACK")));
        }

        [Fact]
        public void A_Single_Value_Written_To_A_Multiple_Outcome_Is_A_Container_Of_One()
        {
            var itemXml = Item(FeedbackMultiple, SetFeedback(@"<qti-base-value base-type=""identifier"">correct</qti-base-value>"));

            var result = Process(itemXml, ResultWithResponse());

            Assert.Equal(new List<string> { "correct" }, Values(ItemOutcome(result, "FEEDBACK")));
        }

        [Theory]
        [InlineData("A", new[] { "always", "correct" })]
        [InlineData("B", new[] { "always", "incorrect" })]
        public void A_Multiple_Outcome_Is_Appended_To_Across_Conditions(string response, string[] expected)
        {
            var itemXml = Item(FeedbackMultiple,
                AppendFeedbackWhen(Always, "always") +
                AppendFeedbackWhen(IsCorrect, "correct") +
                AppendFeedbackWhen(IsIncorrect, "incorrect"));

            var result = Process(itemXml, ResultWithResponse(response));

            Assert.Equal(expected.ToList(), Values(ItemOutcome(result, "FEEDBACK")));
        }

        [Fact]
        public void An_Ordered_Outcome_Keeps_The_Order_It_Was_Built_In()
        {
            var declarations = @"<qti-outcome-declaration identifier=""FEEDBACK"" cardinality=""ordered"" base-type=""identifier""/>";
            var itemXml = Item(declarations,
                AppendFeedbackWhen(Always, "zeta", "qti-ordered") +
                AppendFeedbackWhen(Always, "alpha", "qti-ordered") +
                AppendFeedbackWhen(Always, "mu", "qti-ordered") +
                ScoreWhen(@"<qti-match>
                    <qti-index n=""2""><qti-variable identifier=""FEEDBACK""/></qti-index>
                    <qti-base-value base-type=""identifier"">alpha</qti-base-value>
                </qti-match>"));

            var result = Process(itemXml, ResultWithResponse());

            var feedback = ItemOutcome(result, "FEEDBACK");
            Assert.Equal("ordered", feedback.GetAttributeValue("cardinality"));
            Assert.Equal(new List<string> { "zeta", "alpha", "mu" }, Values(feedback));
            Assert.Equal("1", result.GetScoreForItem(ItemIdentifier, "SCORE"));
        }

        [Fact]
        public void Null_Empties_A_Multiple_Outcome()
        {
            var itemXml = Item(FeedbackMultiple,
                AppendFeedbackWhen(Always, "correct") +
                SetFeedback("<qti-null/>") +
                ScoreWhen(@"<qti-is-null><qti-variable identifier=""FEEDBACK""/></qti-is-null>"));

            var result = Process(itemXml, ResultWithResponse());

            var feedback = ItemOutcome(result, "FEEDBACK");
            Assert.NotNull(feedback);
            Assert.Empty(Values(feedback));
            Assert.Equal("1", result.GetScoreForItem(ItemIdentifier, "SCORE"));
        }

        [Fact]
        public void A_Multiple_Outcome_That_Is_Never_Set_Is_Null()
        {
            // only read, never written: it starts at its (missing) default, which is NULL
            var itemXml = Item(FeedbackMultiple,
                ScoreWhen(@"<qti-and>
                    <qti-is-null><qti-variable identifier=""FEEDBACK""/></qti-is-null>
                    <qti-not><qti-member><qti-base-value base-type=""identifier"">correct</qti-base-value><qti-variable identifier=""FEEDBACK""/></qti-member></qti-not>
                    <qti-not><qti-contains><qti-variable identifier=""FEEDBACK""/><qti-multiple><qti-base-value base-type=""identifier"">correct</qti-base-value></qti-multiple></qti-contains></qti-not>
                    <qti-not><qti-match><qti-multiple><qti-base-value base-type=""identifier"">correct</qti-base-value></qti-multiple><qti-variable identifier=""FEEDBACK""/></qti-match></qti-not>
                </qti-and>"));

            var result = Process(itemXml, ResultWithResponse());

            Assert.Equal("1", result.GetScoreForItem(ItemIdentifier, "SCORE"));
        }

        [Fact]
        public void A_Multiple_Outcome_Starts_At_Its_Declared_Default()
        {
            var declarations = @"<qti-outcome-declaration identifier=""FEEDBACK"" cardinality=""multiple"" base-type=""identifier"">
                    <qti-default-value>
                        <qti-value>seen</qti-value>
                        <qti-value>answered</qti-value>
                    </qti-default-value>
                </qti-outcome-declaration>";
            var itemXml = Item(declarations,
                AppendFeedbackWhen(IsCorrect, "correct") +
                ScoreWhen(@"<qti-equal tolerance-mode=""exact"">
                    <qti-container-size><qti-default identifier=""FEEDBACK""/></qti-container-size>
                    <qti-base-value base-type=""integer"">2</qti-base-value>
                </qti-equal>"));

            var result = Process(itemXml, ResultWithResponse());

            Assert.Equal(new List<string> { "seen", "answered", "correct" }, Values(ItemOutcome(result, "FEEDBACK")));
            Assert.Equal("1", result.GetScoreForItem(ItemIdentifier, "SCORE"));
        }

        [Theory]
        [InlineData(@"<qti-member><qti-base-value base-type=""identifier"">correct</qti-base-value><qti-variable identifier=""FEEDBACK""/></qti-member>")]
        [InlineData(@"<qti-contains><qti-variable identifier=""FEEDBACK""/><qti-multiple><qti-base-value base-type=""identifier"">well_done</qti-base-value><qti-base-value base-type=""identifier"">correct</qti-base-value></qti-multiple></qti-contains>")]
        [InlineData(@"<qti-match><qti-variable identifier=""FEEDBACK""/><qti-multiple><qti-base-value base-type=""identifier"">well_done</qti-base-value><qti-base-value base-type=""identifier"">correct</qti-base-value></qti-multiple></qti-match>")]
        [InlineData(@"<qti-equal tolerance-mode=""exact""><qti-container-size><qti-variable identifier=""FEEDBACK""/></qti-container-size><qti-base-value base-type=""integer"">2</qti-base-value></qti-equal>")]
        public void Container_Operators_Read_A_Multiple_Outcome(string condition)
        {
            var itemXml = Item(FeedbackMultiple,
                AppendFeedbackWhen(IsCorrect, "correct") +
                AppendFeedbackWhen(Always, "well_done") +
                ScoreWhen(condition));

            var result = Process(itemXml, ResultWithResponse());

            Assert.Equal("1", result.GetScoreForItem(ItemIdentifier, "SCORE"));
        }

        [Fact]
        public void A_Multiple_Outcome_RoundTrips_Through_The_Result_And_A_Rescore()
        {
            // TAGS is in the result but not written by the response processing, so it must be
            // read from the result and survive the rescore as it is.
            var declarations = FeedbackMultiple +
                @"<qti-outcome-declaration identifier=""TAGS"" cardinality=""multiple"" base-type=""identifier""/>";
            var itemXml = Item(declarations,
                AppendFeedbackWhen(IsCorrect, "correct") +
                AppendFeedbackWhen(Always, "well_done") +
                ScoreWhen(@"<qti-member><qti-base-value base-type=""identifier"">reviewed</qti-base-value><qti-variable identifier=""TAGS""/></qti-member>"));
            var assessmentResult = XDocument.Parse($@"
                <assessmentResult xmlns=""http://www.imsglobal.org/xsd/imsqti_result_v2p2"">
                    <context sourcedId=""900001""/>
                    <itemResult identifier=""{ItemIdentifier}"" datestamp=""2026-10-09T10:00:00"" sessionStatus=""final"">
                        <responseVariable identifier=""RESPONSE"" cardinality=""single"" baseType=""identifier"">
                            <candidateResponse><value>A</value></candidateResponse>
                        </responseVariable>
                        <outcomeVariable identifier=""TAGS"" cardinality=""multiple"" baseType=""identifier"">
                            <value>flagged</value>
                            <value>reviewed</value>
                        </outcomeVariable>
                    </itemResult>
                </assessmentResult>");

            var scored = ScoreWithEngine(itemXml, assessmentResult);
            var rescored = ScoreWithEngine(itemXml, scored);

            foreach (var result in new[] { scored, rescored })
            {
                var feedback = result.FindElementsByName("outcomeVariable").Where(o => o.Identifier() == "FEEDBACK").ToList();
                Assert.Single(feedback);
                Assert.Equal(new List<string> { "correct", "well_done" }, Values(feedback[0]));
                Assert.Equal(new List<string> { "flagged", "reviewed" }, Values(ItemOutcome(result, "TAGS")));
                Assert.Equal("1", result.GetScoreForItem(ItemIdentifier, "SCORE"));
            }
        }

        [Fact]
        public void Outcome_Processing_Reads_And_Writes_Container_Outcomes()
        {
            var itemXml = Item(FeedbackMultiple, AppendFeedbackWhen(IsCorrect, "correct"));
            var testXml = $@"<qti-assessment-test xmlns=""http://www.imsglobal.org/xsd/imsqtiasi_v3p0"" identifier=""{TestIdentifier}"" title=""test"">
                    <qti-outcome-declaration identifier=""TEST_FEEDBACK"" cardinality=""multiple"" base-type=""identifier""/>
                    <qti-outcome-declaration identifier=""ALL_CORRECT"" cardinality=""single"" base-type=""float"">
                        <qti-default-value><qti-value>0</qti-value></qti-default-value>
                    </qti-outcome-declaration>
                    <qti-test-part identifier=""P1"" navigation-mode=""linear"" submission-mode=""individual"">
                        <qti-assessment-section identifier=""S1"" title=""s"" visible=""true"">
                            <qti-assessment-item-ref identifier=""{ItemIdentifier}"" href=""item.xml""/>
                        </qti-assessment-section>
                    </qti-test-part>
                    <qti-outcome-processing>
                        <qti-set-outcome-value identifier=""TEST_FEEDBACK"">
                            <qti-multiple>
                                <qti-variable identifier=""{ItemIdentifier}.FEEDBACK""/>
                                <qti-base-value base-type=""identifier"">finished</qti-base-value>
                            </qti-multiple>
                        </qti-set-outcome-value>
                        <qti-outcome-condition>
                            <qti-outcome-if>
                                <qti-member>
                                    <qti-base-value base-type=""identifier"">correct</qti-base-value>
                                    <qti-variable identifier=""{ItemIdentifier}.FEEDBACK""/>
                                </qti-member>
                                <qti-set-outcome-value identifier=""ALL_CORRECT""><qti-base-value base-type=""float"">1</qti-base-value></qti-set-outcome-value>
                            </qti-outcome-if>
                        </qti-outcome-condition>
                    </qti-outcome-processing>
                </qti-assessment-test>";
            var assessmentResult = (XDocument)ResultWithResponse();

            var scored = new ScoringEngine().ProcessResponsesAndOutcomes(new ScoringContext
            {
                AssessmentItems = new List<XDocument> { XDocument.Parse(itemXml) },
                AssessmentTest = XDocument.Parse(testXml),
                AssessmentmentResults = new List<XDocument> { assessmentResult },
                Logger = new Mock<ILogger>().Object
            })[0];

            Assert.Equal(new List<string> { "correct", "finished" }, Values(OutcomeElement(scored, "testResult", TestIdentifier, "TEST_FEEDBACK")));
            Assert.Equal("1", OutcomeElement(scored, "testResult", TestIdentifier, "ALL_CORRECT").Value);
        }
    }
}
