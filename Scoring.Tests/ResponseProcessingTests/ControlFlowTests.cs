using Citolab.QTI.ScoringEngine.Model;
using Citolab.QTI.ScoringEngine.ResponseProcessing;
using Microsoft.Extensions.Logging;
using Moq;
using System.Xml.Linq;
using Xunit;
using ILogger = Microsoft.Extensions.Logging.ILogger;

namespace Citolab.QTI.ScoringEngine.Tests.ResponseProcessingTests
{
    /// <summary>
    /// Rules that steer how the response processing itself runs, checked end to end because
    /// they only mean something in the context of the rules around them.
    /// </summary>
    public class ControlFlowTests
    {
        private const string ItemIdentifier = "ITM-CONTROL-FLOW";

        private static string Item(string declarations, string responseProcessing) =>
            $@"<qti-assessment-item xmlns=""http://www.imsglobal.org/xsd/imsqtiasi_v3p0"" identifier=""{ItemIdentifier}"" title=""test"">
                <qti-response-declaration identifier=""RESPONSE"" cardinality=""single"" base-type=""identifier"">
                    <qti-correct-response><qti-value>A</qti-value></qti-correct-response>
                    <qti-default-value><qti-value>Z</qti-value></qti-default-value>
                </qti-response-declaration>
                <qti-outcome-declaration identifier=""SCORE"" cardinality=""single"" base-type=""float"">
                    <qti-default-value><qti-value>0</qti-value></qti-default-value>
                </qti-outcome-declaration>
                {declarations}
                <qti-response-processing>{responseProcessing}</qti-response-processing>
            </qti-assessment-item>";

        private static string SetScore(string score) =>
            $@"<qti-set-outcome-value identifier=""SCORE""><qti-base-value base-type=""float"">{score}</qti-base-value></qti-set-outcome-value>";

        private static AssessmentResult Process(string itemXml, string response)
        {
            var logger = new Mock<ILogger>().Object;
            var assessmentItem = new AssessmentItem(logger, XDocument.Parse(itemXml), TestHelper.GetExpressionFactory());
            var assessmentResult = TestHelper.GetBasicAssessmentResult()
                .AddCandidateResponse(ItemIdentifier, "RESPONSE", response, BaseType.Identifier, Cardinality.Single);
            return ResponseProcessor.Process(assessmentItem, assessmentResult, logger);
        }

        [Fact]
        public void ExitResponse_Skips_The_Rules_That_Follow()
        {
            var itemXml = Item("", $@"
                <qti-response-condition>
                    <qti-response-if>
                        <qti-match><qti-variable identifier=""RESPONSE""/><qti-correct identifier=""RESPONSE""/></qti-match>
                        {SetScore("1")}
                        <qti-exit-response/>
                    </qti-response-if>
                </qti-response-condition>
                {SetScore("99")}");

            var correct = Process(itemXml, "A");
            var incorrect = Process(itemXml, "B");

            Assert.Equal("1", correct.GetScoreForItem(ItemIdentifier, "SCORE"));
            Assert.Equal("99", incorrect.GetScoreForItem(ItemIdentifier, "SCORE"));
        }

        [Fact]
        public void ResponseProcessingFragment_Is_Processed_In_Place()
        {
            var itemXml = Item("", $@"
                <qti-response-processing-fragment>
                    <qti-response-condition>
                        <qti-response-if>
                            <qti-match><qti-variable identifier=""RESPONSE""/><qti-correct identifier=""RESPONSE""/></qti-match>
                            {SetScore("1")}
                        </qti-response-if>
                        <qti-response-else>
                            {SetScore("0")}
                        </qti-response-else>
                    </qti-response-condition>
                </qti-response-processing-fragment>");

            Assert.Equal("1", Process(itemXml, "A").GetScoreForItem(ItemIdentifier, "SCORE"));
            Assert.Equal("0", Process(itemXml, "B").GetScoreForItem(ItemIdentifier, "SCORE"));
        }

        [Fact]
        public void MatchTable_Maps_The_Score_To_Its_Target_Value()
        {
            var gradeDeclaration = @"<qti-outcome-declaration identifier=""GRADE"" cardinality=""single"" base-type=""string"">
                    <qti-match-table>
                        <qti-match-table-entry source-value=""1"" target-value=""pass""/>
                        <qti-match-table-entry source-value=""0"" target-value=""fail""/>
                    </qti-match-table>
                </qti-outcome-declaration>";
            var itemXml = Item(gradeDeclaration, $@"
                <qti-response-condition>
                    <qti-response-if>
                        <qti-match><qti-variable identifier=""RESPONSE""/><qti-correct identifier=""RESPONSE""/></qti-match>
                        {SetScore("1")}
                    </qti-response-if>
                    <qti-response-else>
                        {SetScore("0")}
                    </qti-response-else>
                </qti-response-condition>
                <qti-lookup-outcome-value identifier=""GRADE""><qti-variable identifier=""SCORE""/></qti-lookup-outcome-value>");

            Assert.Equal("pass", Process(itemXml, "A").GetScoreForItem(ItemIdentifier, "GRADE"));
            Assert.Equal("fail", Process(itemXml, "B").GetScoreForItem(ItemIdentifier, "GRADE"));
        }

        [Fact]
        public void Default_Returns_The_Declared_Default_Value()
        {
            var itemXml = Item("", @"
                <qti-response-condition>
                    <qti-response-if>
                        <qti-match><qti-variable identifier=""RESPONSE""/><qti-default identifier=""RESPONSE""/></qti-match>
                        <qti-set-outcome-value identifier=""SCORE""><qti-base-value base-type=""float"">1</qti-base-value></qti-set-outcome-value>
                    </qti-response-if>
                </qti-response-condition>");

            // Z is the default of RESPONSE, so only that response matches qti-default
            Assert.Equal("1", Process(itemXml, "Z").GetScoreForItem(ItemIdentifier, "SCORE"));
            Assert.Equal("0", Process(itemXml, "A").GetScoreForItem(ItemIdentifier, "SCORE"));
        }
    }
}
