using Citolab.QTI.ScoringEngine.Helpers;
using Citolab.QTI.ScoringEngine.Model;
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
    /// Adaptive items (adaptive="true"): the outcomes keep their values from one attempt to the next. A delivery
    /// system scores attempt by attempt and passes the itemResult of the previous attempt - its outcomeVariables -
    /// with the responses of the new one.
    /// </summary>
    public class AdaptiveItemTests
    {
        private const string ItemIdentifier = "ITM-ADAPTIVE";
        private static readonly XNamespace ResultNs = "http://www.imsglobal.org/xsd/imsqti_result_v2p2";

        /// <summary>
        /// A question with a follow-up question: step 1 is a moment in a video (record response, right between 2
        /// and 3 seconds), step 2 a choice (A is right) that is shown once step 1 has been answered. STEP picks the
        /// branch, so a step is scored in the attempt that ends it and never again.
        /// </summary>
        private static string Item(bool adaptive = true) =>
            $@"<qti-assessment-item xmlns=""http://www.imsglobal.org/xsd/imsqtiasi_v3p0"" identifier=""{ItemIdentifier}"" title=""test""
                adaptive=""{(adaptive ? "true" : "false")}"" time-dependent=""false"">
                <qti-response-declaration identifier=""RESPONSE"" cardinality=""record""/>
                <qti-response-declaration identifier=""RESPONSE2"" cardinality=""single"" base-type=""identifier"">
                    <qti-correct-response><qti-value>A</qti-value></qti-correct-response>
                </qti-response-declaration>
                <qti-outcome-declaration identifier=""SCORE"" cardinality=""single"" base-type=""float""><qti-default-value><qti-value>0</qti-value></qti-default-value></qti-outcome-declaration>
                <qti-outcome-declaration identifier=""SCORE_S1"" cardinality=""single"" base-type=""float""><qti-default-value><qti-value>0</qti-value></qti-default-value></qti-outcome-declaration>
                <qti-outcome-declaration identifier=""SCORE_S2"" cardinality=""single"" base-type=""float""><qti-default-value><qti-value>0</qti-value></qti-default-value></qti-outcome-declaration>
                <qti-outcome-declaration identifier=""STEP"" cardinality=""single"" base-type=""integer""><qti-default-value><qti-value>1</qti-value></qti-default-value></qti-outcome-declaration>
                <qti-outcome-declaration identifier=""STEP_2"" cardinality=""single"" base-type=""identifier""/>
                <qti-item-body/>
                <qti-response-processing>
                    <qti-response-condition>
                        <qti-response-if>
                            <qti-match><qti-variable identifier=""STEP""/><qti-base-value base-type=""integer"">1</qti-base-value></qti-match>
                            <qti-response-condition>
                                <qti-response-if>
                                    <qti-and>
                                        <qti-gte><qti-field-value field-identifier=""time""><qti-variable identifier=""RESPONSE""/></qti-field-value><qti-base-value base-type=""float"">2</qti-base-value></qti-gte>
                                        <qti-lte><qti-field-value field-identifier=""time""><qti-variable identifier=""RESPONSE""/></qti-field-value><qti-base-value base-type=""float"">3</qti-base-value></qti-lte>
                                    </qti-and>
                                    <qti-set-outcome-value identifier=""SCORE_S1""><qti-base-value base-type=""float"">1</qti-base-value></qti-set-outcome-value>
                                </qti-response-if>
                            </qti-response-condition>
                            <qti-set-outcome-value identifier=""STEP_2""><qti-base-value base-type=""identifier"">SHOW</qti-base-value></qti-set-outcome-value>
                            <qti-set-outcome-value identifier=""STEP""><qti-base-value base-type=""integer"">2</qti-base-value></qti-set-outcome-value>
                        </qti-response-if>
                        <qti-response-else-if>
                            <qti-match><qti-variable identifier=""STEP""/><qti-base-value base-type=""integer"">2</qti-base-value></qti-match>
                            <qti-response-condition>
                                <qti-response-if>
                                    <qti-match><qti-variable identifier=""RESPONSE2""/><qti-correct identifier=""RESPONSE2""/></qti-match>
                                    <qti-set-outcome-value identifier=""SCORE_S2""><qti-base-value base-type=""float"">1</qti-base-value></qti-set-outcome-value>
                                </qti-response-if>
                            </qti-response-condition>
                            <qti-set-outcome-value identifier=""STEP""><qti-base-value base-type=""integer"">3</qti-base-value></qti-set-outcome-value>
                            <qti-set-outcome-value identifier=""completionStatus""><qti-base-value base-type=""identifier"">completed</qti-base-value></qti-set-outcome-value>
                        </qti-response-else-if>
                    </qti-response-condition>
                    <qti-set-outcome-value identifier=""SCORE"">
                        <qti-sum><qti-variable identifier=""SCORE_S1""/><qti-variable identifier=""SCORE_S2""/></qti-sum>
                    </qti-set-outcome-value>
                </qti-response-processing>
            </qti-assessment-item>";

        private static XElement Responses(string time, string choice) => new XElement(ResultNs + "itemResult",
            new XElement(ResultNs + "responseVariable",
                new XAttribute("identifier", "RESPONSE"), new XAttribute("cardinality", "record"),
                new XElement(ResultNs + "candidateResponse", time == null ? null :
                    new XElement(ResultNs + "value", new XAttribute("fieldIdentifier", "time"), new XAttribute("baseType", "float"), time))),
            new XElement(ResultNs + "responseVariable",
                new XAttribute("identifier", "RESPONSE2"), new XAttribute("cardinality", "single"), new XAttribute("baseType", "identifier"),
                new XElement(ResultNs + "candidateResponse", choice == null ? null : new XElement(ResultNs + "value", choice))));

        /// <summary>
        /// Scores the attempts one after the other. Each attempt gets the itemResult the previous one produced, with
        /// its responses replaced by the new ones. Returns the scored result of every attempt.
        /// </summary>
        private static List<XDocument> Attempts(string itemXml, params (string Time, string Choice)[] attempts)
        {
            var engine = new ScoringEngine();
            var item = XDocument.Parse(itemXml);
            var scored = new List<XDocument>();
            XDocument previous = null;
            foreach (var (time, choice) in attempts)
            {
                var itemResult = previous == null
                    ? new XElement(ResultNs + "itemResult")
                    : new XElement(previous.Root.Elements(ResultNs + "itemResult").Single());
                itemResult.SetAttributeValue("identifier", ItemIdentifier);
                itemResult.SetAttributeValue("datestamp", "2026-10-15T10:00:00");
                itemResult.SetAttributeValue("sessionStatus", "final");
                itemResult.Elements(ResultNs + "responseVariable").Remove();
                itemResult.AddFirst(Responses(time, choice).Elements());

                var result = new XDocument(new XElement(ResultNs + "assessmentResult",
                    new XElement(ResultNs + "context", new XAttribute("sourcedId", "900001")),
                    itemResult));
                previous = engine.ProcessResponses(new ResponseProcessingContext
                {
                    AssessmentItems = new List<XDocument> { item },
                    AssessmentmentResults = new List<XDocument> { result },
                    Logger = new Mock<ILogger>().Object
                }).Single();
                scored.Add(previous);
            }
            return scored;
        }

        private static string Outcome(XDocument result, string identifier) =>
            result.GetScoreForItem(ItemIdentifier, identifier);

        [Fact]
        public void The_Outcomes_Of_An_Attempt_Are_Where_The_Next_Attempt_Starts()
        {
            var results = Attempts(Item(), ("2.5", null), (null, "A"));

            Assert.Equal("2", Outcome(results[0], "STEP"));
            Assert.Equal("SHOW", Outcome(results[0], "STEP_2"));
            Assert.Equal("1", Outcome(results[0], "SCORE"));
            Assert.Equal("3", Outcome(results[1], "STEP"));
            Assert.Equal("1", Outcome(results[1], "SCORE_S1"));
            Assert.Equal("1", Outcome(results[1], "SCORE_S2"));
            Assert.Equal("2", Outcome(results[1], "SCORE"));
        }

        [Theory]
        [InlineData("2.5", "4.0", "1")] // right first, then changed to wrong: stays right
        [InlineData("4.0", "2.5", "0")] // wrong first, then changed to right after seeing step 2: stays wrong
        public void A_Step_Is_Scored_Once_And_A_Later_Change_Of_Its_Response_Is_Ignored(string first, string later, string scoreS1)
        {
            var results = Attempts(Item(), (first, null), (later, "B"));

            Assert.Equal(scoreS1, Outcome(results[1], "SCORE_S1"));
            Assert.Equal("0", Outcome(results[1], "SCORE_S2"));
            Assert.Equal(scoreS1, Outcome(results[1], "SCORE"));
        }

        [Fact]
        public void CompletionStatus_Is_An_Identifier_Set_By_The_Response_Processing()
        {
            var results = Attempts(Item(), ("2.5", null), ("2.5", "A"));

            Assert.Equal("unknown", Outcome(results[0], "completionStatus"));
            Assert.Equal("completed", Outcome(results[1], "completionStatus"));
            var variable = results[1].Root.Descendants()
                .Single(v => v.Name.LocalName == "outcomeVariable" && v.Attribute("identifier")?.Value == "completionStatus");
            Assert.Equal("identifier", variable.Attribute("baseType")?.Value);
        }

        [Fact]
        public void An_Attempt_After_The_Item_Was_Completed_Changes_Nothing()
        {
            var results = Attempts(Item(), ("2.5", null), ("2.5", "A"), ("4.0", "B"));

            Assert.Equal("3", Outcome(results[2], "STEP"));
            Assert.Equal("2", Outcome(results[2], "SCORE"));
            Assert.Equal("completed", Outcome(results[2], "completionStatus"));
        }

        [Fact]
        public void A_Non_Adaptive_Item_Still_Recalculates_Its_Outcomes_From_Their_Defaults()
        {
            // Same item, adaptive="false": every attempt starts at STEP 1, so step 1 is scored again each time.
            var results = Attempts(Item(adaptive: false), ("2.5", null), ("4.0", "A"));

            Assert.Equal("2", Outcome(results[1], "STEP"));
            Assert.Equal("0", Outcome(results[1], "SCORE_S1"));
            Assert.Equal("0", Outcome(results[1], "SCORE"));
        }

        [Fact]
        public void NumAttempts_Passed_As_A_Response_Variable_Can_Be_Read()
        {
            var logger = new Mock<ILogger>().Object;
            var itemXml = $@"<qti-assessment-item xmlns=""http://www.imsglobal.org/xsd/imsqtiasi_v3p0"" identifier=""{ItemIdentifier}"" title=""test"" adaptive=""true"">
                <qti-response-declaration identifier=""RESPONSE"" cardinality=""single"" base-type=""identifier""/>
                <qti-outcome-declaration identifier=""SCORE"" cardinality=""single"" base-type=""float""><qti-default-value><qti-value>0</qti-value></qti-default-value></qti-outcome-declaration>
                <qti-response-processing>
                    <qti-response-condition>
                        <qti-response-if>
                            <qti-gte><qti-variable identifier=""numAttempts""/><qti-base-value base-type=""integer"">2</qti-base-value></qti-gte>
                            <qti-set-outcome-value identifier=""SCORE""><qti-base-value base-type=""float"">1</qti-base-value></qti-set-outcome-value>
                        </qti-response-if>
                    </qti-response-condition>
                </qti-response-processing>
            </qti-assessment-item>";
            var assessmentItem = new AssessmentItem(logger, XDocument.Parse(itemXml), TestHelper.GetExpressionFactory());

            // numAttempts as a delivery system reports it: a response variable of the itemResult.
            var assessmentResult = new AssessmentResult(logger, XDocument.Parse($@"
                <assessmentResult xmlns=""http://www.imsglobal.org/xsd/imsqti_result_v2p2"">
                    <context sourcedId=""900001""/>
                    <itemResult identifier=""{ItemIdentifier}"" datestamp=""2026-10-15T10:00:00"" sessionStatus=""final"">
                        <responseVariable identifier=""numAttempts"" cardinality=""single"" baseType=""integer"">
                            <candidateResponse><value>2</value></candidateResponse>
                        </responseVariable>
                        <responseVariable identifier=""RESPONSE"" cardinality=""single"" baseType=""identifier"">
                            <candidateResponse><value>A</value></candidateResponse>
                        </responseVariable>
                    </itemResult>
                </assessmentResult>"));

            var result = ResponseProcessing.ResponseProcessor.Process(assessmentItem, assessmentResult, logger);

            Assert.Equal("1", result.GetScoreForItem(ItemIdentifier, "SCORE"));
        }

        [Fact]
        public void An_Outcome_Without_A_Value_Is_Null()
        {
            var logger = new Mock<ILogger>().Object;
            var itemXml = $@"<qti-assessment-item xmlns=""http://www.imsglobal.org/xsd/imsqtiasi_v3p0"" identifier=""{ItemIdentifier}"" title=""test"">
                <qti-response-declaration identifier=""RESPONSE"" cardinality=""single"" base-type=""identifier""/>
                <qti-outcome-declaration identifier=""SCORE"" cardinality=""single"" base-type=""float""><qti-default-value><qti-value>0</qti-value></qti-default-value></qti-outcome-declaration>
                <qti-outcome-declaration identifier=""UNSET"" cardinality=""single"" base-type=""identifier""/>
                <qti-outcome-declaration identifier=""SET"" cardinality=""single"" base-type=""identifier""><qti-default-value><qti-value>X</qti-value></qti-default-value></qti-outcome-declaration>
                <qti-response-processing>
                    <qti-response-condition>
                        <qti-response-if>
                            <qti-and>
                                <qti-is-null><qti-variable identifier=""UNSET""/></qti-is-null>
                                <qti-not><qti-is-null><qti-variable identifier=""SET""/></qti-is-null></qti-not>
                            </qti-and>
                            <qti-set-outcome-value identifier=""SCORE""><qti-base-value base-type=""float"">1</qti-base-value></qti-set-outcome-value>
                        </qti-response-if>
                    </qti-response-condition>
                </qti-response-processing>
            </qti-assessment-item>";
            var assessmentItem = new AssessmentItem(logger, XDocument.Parse(itemXml), TestHelper.GetExpressionFactory());

            var result = ResponseProcessing.ResponseProcessor.Process(assessmentItem,
                TestHelper.GetBasicAssessmentResult().AddCandidateResponse(ItemIdentifier, "RESPONSE", "A", BaseType.Identifier, Cardinality.Single),
                logger);

            Assert.Equal("1", result.GetScoreForItem(ItemIdentifier, "SCORE"));
        }
    }
}
