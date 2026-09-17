using Citolab.QTI.ScoringEngine.Helpers;
using Citolab.QTI.ScoringEngine.Interfaces;
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
    /// Record cardinality: QTI_CONTEXT, record variables in the result and qti-field-value.
    /// </summary>
    public class RecordAndContextTests
    {
        private const string ItemIdentifier = "ITM-RECORD";

        private static string Item(string declarations, string responseProcessing) =>
            $@"<qti-assessment-item xmlns=""http://www.imsglobal.org/xsd/imsqtiasi_v3p0"" identifier=""{ItemIdentifier}"" title=""test"">
                <qti-response-declaration identifier=""RESPONSE"" cardinality=""single"" base-type=""identifier""/>
                <qti-outcome-declaration identifier=""SCORE"" cardinality=""single"" base-type=""float"">
                    <qti-default-value><qti-value>0</qti-value></qti-default-value>
                </qti-outcome-declaration>
                {declarations}
                <qti-response-processing>{responseProcessing}</qti-response-processing>
            </qti-assessment-item>";

        /// <summary>
        /// Scores 1 when the expression is true, so a test can assert on a single score.
        /// </summary>
        private static string ScoreWhen(string condition) =>
            $@"<qti-response-condition>
                    <qti-response-if>
                        {condition}
                        <qti-set-outcome-value identifier=""SCORE""><qti-base-value base-type=""float"">1</qti-base-value></qti-set-outcome-value>
                    </qti-response-if>
                </qti-response-condition>";

        private static AssessmentResult Process(string itemXml, AssessmentResult assessmentResult, ResponseProcessingScoringsOptions options = null)
        {
            var logger = new Mock<ILogger>().Object;
            var assessmentItem = new AssessmentItem(logger, XDocument.Parse(itemXml), TestHelper.GetExpressionFactory());
            return ResponseProcessor.Process(assessmentItem, assessmentResult, logger, options);
        }

        private static AssessmentResult ResultWithResponse() =>
            TestHelper.GetBasicAssessmentResult()
                .AddCandidateResponse(ItemIdentifier, "RESPONSE", "A", BaseType.Identifier, Cardinality.Single);

        /// <summary>
        /// A result holding a record response variable, as a delivery engine writes it.
        /// </summary>
        private static AssessmentResult ResultWithRecordResponse()
        {
            var logger = new Mock<ILogger>().Object;
            return new AssessmentResult(logger, XDocument.Parse($@"
                <assessmentResult xmlns=""http://www.imsglobal.org/xsd/imsqti_result_v2p2"">
                    <context sourcedId=""900001"" environmentIdentifier=""ENV_1""/>
                    <itemResult identifier=""{ItemIdentifier}"" datestamp=""2026-09-17T10:00:00"" sessionStatus=""final"">
                        <responseVariable identifier=""RESPONSE"" cardinality=""record"">
                            <candidateResponse>
                                <value fieldIdentifier=""stringValue"" baseType=""string"">hello</value>
                                <value fieldIdentifier=""floatValue"" baseType=""float"">1.5</value>
                            </candidateResponse>
                        </responseVariable>
                    </itemResult>
                </assessmentResult>"));
        }

        [Fact]
        public void FieldValue_Reads_A_Field_Of_QtiContext()
        {
            var itemXml = Item("", ScoreWhen(@"<qti-match>
                    <qti-field-value field-identifier=""candidateIdentifier""><qti-variable identifier=""QTI_CONTEXT""/></qti-field-value>
                    <qti-base-value base-type=""identifier"">900001</qti-base-value>
                </qti-match>"));

            var result = Process(itemXml, ResultWithResponse());

            Assert.Equal("1", result.GetScoreForItem(ItemIdentifier, "SCORE"));
        }

        [Fact]
        public void FieldValue_Of_A_Field_That_Is_Not_There_Is_Null()
        {
            var itemXml = Item("", ScoreWhen(@"<qti-is-null>
                    <qti-field-value field-identifier=""environmentIdentifier""><qti-variable identifier=""QTI_CONTEXT""/></qti-field-value>
                </qti-is-null>"));

            var result = Process(itemXml, ResultWithResponse());

            Assert.Equal("1", result.GetScoreForItem(ItemIdentifier, "SCORE"));
        }

        [Fact]
        public void QtiContext_Takes_The_Fields_The_Caller_Provides()
        {
            var itemXml = Item("", ScoreWhen(@"<qti-match>
                    <qti-field-value field-identifier=""environmentIdentifier""><qti-variable identifier=""QTI_CONTEXT""/></qti-field-value>
                    <qti-base-value base-type=""identifier"">ENV_1</qti-base-value>
                </qti-match>"));
            var options = new ResponseProcessingScoringsOptions
            {
                QtiContextFields = new Dictionary<string, string> { { "environmentIdentifier", "ENV_1" } }
            };

            var result = Process(itemXml, ResultWithResponse(), options);

            Assert.Equal("1", result.GetScoreForItem(ItemIdentifier, "SCORE"));
        }

        [Fact]
        public void QtiContext_Reads_The_EnvironmentIdentifier_Of_The_Result()
        {
            var itemXml = Item("", ScoreWhen(@"<qti-match>
                    <qti-field-value field-identifier=""environmentIdentifier""><qti-variable identifier=""QTI_CONTEXT""/></qti-field-value>
                    <qti-base-value base-type=""identifier"">ENV_1</qti-base-value>
                </qti-match>"));

            var result = Process(itemXml, ResultWithRecordResponse());

            Assert.Equal("1", result.GetScoreForItem(ItemIdentifier, "SCORE"));
        }

        [Fact]
        public void FieldValue_Reads_A_Field_Of_A_Record_Response()
        {
            var itemXml = Item("", @"<qti-set-outcome-value identifier=""SCORE"">
                    <qti-field-value field-identifier=""floatValue""><qti-variable identifier=""RESPONSE""/></qti-field-value>
                </qti-set-outcome-value>");

            var result = Process(itemXml, ResultWithRecordResponse());

            Assert.Equal("1.5", result.GetScoreForItem(ItemIdentifier, "SCORE"));
        }

        [Fact]
        public void FieldValue_On_Something_That_Is_Not_A_Record_Is_Null()
        {
            var itemXml = Item("", ScoreWhen(@"<qti-is-null>
                    <qti-field-value field-identifier=""floatValue""><qti-variable identifier=""RESPONSE""/></qti-field-value>
                </qti-is-null>"));

            var result = Process(itemXml, ResultWithResponse());

            Assert.Equal("1", result.GetScoreForItem(ItemIdentifier, "SCORE"));
        }

        [Fact]
        public void A_Record_Outcome_Is_Written_Back_To_The_Result()
        {
            var itemXml = Item(@"<qti-outcome-declaration identifier=""CONTEXT_COPY"" cardinality=""record""/>",
                @"<qti-set-outcome-value identifier=""CONTEXT_COPY""><qti-variable identifier=""QTI_CONTEXT""/></qti-set-outcome-value>");

            var result = Process(itemXml, ResultWithResponse());

            var outcomeVariable = result
                .FindElementsByElementAndAttributeValue("itemResult", "identifier", ItemIdentifier)
                .FirstOrDefault()
                .FindElementsByElementAndAttributeValue("outcomeVariable", "identifier", "CONTEXT_COPY")
                .FirstOrDefault();
            Assert.Equal("record", outcomeVariable.GetAttributeValue("cardinality"));
            var candidateField = outcomeVariable
                .FindElementsByElementAndAttributeValue("value", "fieldIdentifier", "candidateIdentifier")
                .FirstOrDefault();
            Assert.Equal("900001", candidateField.Value);
            Assert.Equal("identifier", candidateField.GetAttributeValue("baseType"));
        }

        [Fact]
        public void FieldValue_Reads_A_Record_A_CustomOperator_Returned()
        {
            var logger = new Mock<ILogger>().Object;
            var expressionFactory = new ExpressionFactory(
                new Dictionary<string, ICustomOperator> { { "test:Record", new RecordOperator() } }, logger);
            var itemXml = Item("", @"<qti-set-outcome-value identifier=""SCORE"">
                    <qti-field-value field-identifier=""factor"">
                        <qti-custom-operator definition=""test:Record""><qti-variable identifier=""RESPONSE""/></qti-custom-operator>
                    </qti-field-value>
                </qti-set-outcome-value>");
            var assessmentItem = new AssessmentItem(logger, XDocument.Parse(itemXml), expressionFactory);

            var result = ResponseProcessor.Process(assessmentItem, ResultWithResponse(), logger);

            Assert.Equal("2.5", result.GetScoreForItem(ItemIdentifier, "SCORE"));
        }

        [Fact]
        public void Default_Returns_The_Declared_Record()
        {
            var declarations = @"<qti-outcome-declaration identifier=""CONFIG"" cardinality=""record"">
                    <qti-default-value>
                        <qti-value field-identifier=""factor"" base-type=""float"">0.5</qti-value>
                    </qti-default-value>
                </qti-outcome-declaration>";
            var itemXml = Item(declarations, @"<qti-set-outcome-value identifier=""SCORE"">
                    <qti-field-value field-identifier=""factor""><qti-default identifier=""CONFIG""/></qti-field-value>
                </qti-set-outcome-value>");

            var result = Process(itemXml, ResultWithResponse());

            Assert.Equal("0.5", result.GetScoreForItem(ItemIdentifier, "SCORE"));
        }

        [Fact]
        public void A_Record_Outcome_Starts_At_Its_Declared_Default()
        {
            var declarations = @"<qti-outcome-declaration identifier=""CONFIG"" cardinality=""record"">
                    <qti-default-value>
                        <qti-value field-identifier=""factor"" base-type=""float"">0.5</qti-value>
                    </qti-default-value>
                </qti-outcome-declaration>";
            var itemXml = Item(declarations, @"<qti-set-outcome-value identifier=""SCORE"">
                    <qti-field-value field-identifier=""factor""><qti-variable identifier=""CONFIG""/></qti-field-value>
                </qti-set-outcome-value>");

            var result = Process(itemXml, ResultWithResponse());

            Assert.Equal("0.5", result.GetScoreForItem(ItemIdentifier, "SCORE"));
        }
    }

    /// <summary>
    /// A custom operator that returns a record, the way a delivery engine's own operator can.
    /// </summary>
    internal class RecordOperator : ICustomOperator
    {
        public BaseValue Apply(List<BaseValue> values)
        {
            return new BaseValue
            {
                Identifier = "record",
                BaseType = BaseType.Float,
                Cardinality = Cardinality.Record,
                Fields = new Dictionary<string, BaseValue>
                {
                    { "factor", new BaseValue { Identifier = "factor", BaseType = BaseType.Float, Cardinality = Cardinality.Single, Value = "2.5" } }
                }
            };
        }
    }
}
