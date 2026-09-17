using Citolab.QTI.ScoringEngine.Model;
using Citolab.QTI.ScoringEngine.ResponseProcessing;
using Microsoft.Extensions.Logging;
using Moq;
using System.Xml.Linq;
using Xunit;
using ILogger = Microsoft.Extensions.Logging.ILogger;

namespace Citolab.QTI.ScoringEngine.Tests.ResponseProcessingTests.ScoringTypes
{
    /// <summary>
    /// Mapping a response that holds more than one value, read from an assessmentResult rather
    /// than built by the caller: the result is the only place the values arrive separately.
    /// </summary>
    public class MapResponseCardinalityTests
    {
        private const string ItemIdentifier = "ITM-MAPPING";

        private static string Item(string responseDeclaration, string mapExpression) =>
            $@"<qti-assessment-item xmlns=""http://www.imsglobal.org/xsd/imsqtiasi_v3p0"" identifier=""{ItemIdentifier}"" title=""test"">
                {responseDeclaration}
                <qti-outcome-declaration identifier=""SCORE"" cardinality=""single"" base-type=""float"">
                    <qti-default-value><qti-value>0</qti-value></qti-default-value>
                </qti-outcome-declaration>
                <qti-response-processing>
                    <qti-set-outcome-value identifier=""SCORE"">{mapExpression}</qti-set-outcome-value>
                </qti-response-processing>
            </qti-assessment-item>";

        private static string Result(string cardinality, string baseType, params string[] values)
        {
            var valueElements = string.Join("", System.Array.ConvertAll(values, value => $"<value>{value}</value>"));
            return $@"<assessmentResult xmlns=""http://www.imsglobal.org/xsd/imsqti_result_v2p2"">
                    <context sourcedId=""900001""/>
                    <itemResult identifier=""{ItemIdentifier}"" datestamp=""2026-09-17T10:00:00"" sessionStatus=""final"">
                        <responseVariable identifier=""RESPONSE"" cardinality=""{cardinality}"" baseType=""{baseType}"">
                            <candidateResponse>{valueElements}</candidateResponse>
                        </responseVariable>
                    </itemResult>
                </assessmentResult>";
        }

        private static string Score(string itemXml, string resultXml)
        {
            var logger = new Mock<ILogger>().Object;
            var assessmentItem = new AssessmentItem(logger, XDocument.Parse(itemXml), TestHelper.GetExpressionFactory());
            var assessmentResult = new AssessmentResult(logger, XDocument.Parse(resultXml));

            ResponseProcessor.Process(assessmentItem, assessmentResult, logger);

            return assessmentResult.GetScoreForItem(ItemIdentifier, "SCORE");
        }

        private static string MappedItem(string cardinality) =>
            Item($@"<qti-response-declaration identifier=""RESPONSE"" cardinality=""{cardinality}"" base-type=""identifier"">
                    <qti-mapping default-value=""0"">
                        <qti-map-entry map-key=""A"" mapped-value=""1""/>
                        <qti-map-entry map-key=""B"" mapped-value=""2""/>
                    </qti-mapping>
                </qti-response-declaration>",
                @"<qti-map-response identifier=""RESPONSE""/>");

        private static string AreaMappedItem(string cardinality) =>
            Item($@"<qti-response-declaration identifier=""RESPONSE"" cardinality=""{cardinality}"" base-type=""point"">
                    <qti-area-mapping default-value=""0"">
                        <qti-area-map-entry shape=""circle"" coords=""100,100,15"" mapped-value=""1""/>
                        <qti-area-map-entry shape=""rect"" coords=""200,200,300,300"" mapped-value=""2""/>
                    </qti-area-mapping>
                </qti-response-declaration>",
                @"<qti-map-response-point identifier=""RESPONSE""/>");

        [Fact]
        public void MapResponse_Maps_Every_Value_Of_A_Multiple_Response()
        {
            Assert.Equal("3", Score(MappedItem("multiple"), Result("multiple", "identifier", "A", "B")));
        }

        [Fact]
        public void MapResponse_Maps_Every_Value_Of_An_Ordered_Response()
        {
            Assert.Equal("3", Score(MappedItem("ordered"), Result("ordered", "identifier", "A", "B")));
        }

        [Fact]
        public void MapResponse_Falls_Back_To_The_Default_Of_The_Mapping_Per_Value()
        {
            // B maps to 2, the unmapped C adds the default of 0
            Assert.Equal("2", Score(MappedItem("multiple"), Result("multiple", "identifier", "B", "C")));
        }

        [Fact]
        public void MapResponse_Maps_A_Single_Response()
        {
            Assert.Equal("1", Score(MappedItem("single"), Result("single", "identifier", "A")));
        }

        [Fact]
        public void MapResponsePoint_Maps_Every_Point_Of_A_Multiple_Response()
        {
            Assert.Equal("3", Score(AreaMappedItem("multiple"), Result("multiple", "point", "100 105", "250 250")));
        }

        [Fact]
        public void MapResponsePoint_Maps_A_Single_Response()
        {
            Assert.Equal("1", Score(AreaMappedItem("single"), Result("single", "point", "100 105")));
        }
    }
}
