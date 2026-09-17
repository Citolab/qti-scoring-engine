using Citolab.QTI.ScoringEngine;
using Citolab.QTI.ScoringEngine.Helpers;
using Citolab.QTI.ScoringEngine.Model;
using Citolab.QTI.ScoringEngine.Tests;
using Microsoft.Extensions.Logging;
using Moq;
using System.Collections.Generic;
using System.Linq;
using System.Xml.Linq;
using Xunit;

namespace ScoringEngine.Tests.EngineTests
{
    public class SerializationAndBaseTypeTests
    {
        /// <summary>
        /// An item that copies a literal into a string outcome, so whatever is in
        /// <paramref name="outcomeValue"/> ends up being written to the assessmentResult.
        /// </summary>
        private static XDocument ItemWritingStringOutcome(string outcomeValue) => XDocument.Parse($@"<assessmentItem xmlns=""http://www.imsglobal.org/xsd/imsqti_v2p2"" identifier=""ITM-TEXT"" title=""t"" timeDependent=""false"">
  <responseDeclaration identifier=""RESPONSE"" cardinality=""single"" baseType=""string"">
    <correctResponse><value>test</value></correctResponse>
  </responseDeclaration>
  <outcomeDeclaration identifier=""FEEDBACK"" cardinality=""single"" baseType=""string""/>
  <responseProcessing><responseCondition><responseIf>
    <match><variable identifier=""RESPONSE""/><correct identifier=""RESPONSE""/></match>
    <setOutcomeValue identifier=""FEEDBACK""><baseValue baseType=""string"">{outcomeValue}</baseValue></setOutcomeValue>
  </responseIf></responseCondition></responseProcessing>
</assessmentItem>");

        private static XDocument Score(XDocument item, string itemIdentifier)
        {
            var result = TestHelper.GetBasicAssessmentResult();
            result.AddCandidateResponse(itemIdentifier, "RESPONSE", "test", BaseType.String, Cardinality.Single);
            return new Citolab.QTI.ScoringEngine.ScoringEngine().ProcessResponses(new ResponseProcessingContext
            {
                AssessmentItems = new List<XDocument> { item },
                AssessmentmentResults = new List<XDocument> { result },
                Logger = new Mock<ILogger>().Object
            })[0];
        }

        [Theory]
        // written as XML entities in the item, so these are the decoded values the engine handles
        [InlineData("Jip &amp; Janneke", "Jip & Janneke")]
        [InlineData("2 &lt; 3", "2 < 3")]
        [InlineData("3 &gt; 2", "3 > 2")]
        [InlineData("she said &quot;yes&quot;", "she said \"yes\"")]
        [InlineData("a &amp; b &lt; c &gt; d", "a & b < c > d")]
        public void OutcomeValuesNeedingXmlEscapingRoundTrip(string literalInItem, string expected)
        {
            // These used to be interpolated straight into an XML string and parsed back,
            // so an ampersand or angle bracket in an outcome threw XmlException.
            var scored = Score(ItemWritingStringOutcome(literalInItem), "ITM-TEXT");

            var value = scored.FindElementsByName("outcomeVariable")
                .First(o => o.Identifier() == "FEEDBACK")
                .FindElementsByName("value").First().Value;
            Assert.Equal(expected, value);
        }

        [Theory]
        [InlineData("identifier", BaseType.Identifier, "identifier")]
        [InlineData("float", BaseType.Float, "float")]
        [InlineData("integer", BaseType.Int, "integer")]
        [InlineData("string", BaseType.String, "string")]
        [InlineData("pair", BaseType.Pair, "pair")]
        [InlineData("directedPair", BaseType.DirectedPair, "directedPair")]
        [InlineData("point", BaseType.Point, "point")]
        [InlineData("boolean", BaseType.Boolean, "boolean")]
        [InlineData("duration", BaseType.Duration, "duration")]
        [InlineData("file", BaseType.file, "file")]
        [InlineData("uri", BaseType.Uri, "uri")]
        [InlineData("intOrIdentifier", BaseType.IntOrIdentifier, "intOrIdentifier")]
        public void EveryBaseTypeRoundTripsThroughItsSpecName(string name, BaseType expected, string expectedName)
        {
            Assert.Equal(expected, name.ToBaseType());
            Assert.Equal(expectedName, expected.GetString());
        }

        [Fact]
        public void AnUnknownBaseTypeFallsBackToStringAndIsLogged()
        {
            var log = new Mock<ILogger>();
            Assert.Equal(BaseType.String, "somethingElse".ToBaseType(log.Object));
            log.Verify(l => l.Log(LogLevel.Warning, It.IsAny<EventId>(), It.IsAny<It.IsAnyType>(),
                It.IsAny<System.Exception>(), It.IsAny<System.Func<It.IsAnyType, System.Exception, string>>()), Times.Once);
        }

        [Fact]
        public void ABooleanOutcomeIsWrittenBackAsBoolean()
        {
            var item = XDocument.Parse(@"<assessmentItem xmlns=""http://www.imsglobal.org/xsd/imsqti_v2p2"" identifier=""ITM-BOOL"" title=""t"" timeDependent=""false"">
  <responseDeclaration identifier=""RESPONSE"" cardinality=""single"" baseType=""string"">
    <correctResponse><value>test</value></correctResponse>
  </responseDeclaration>
  <outcomeDeclaration identifier=""PASSED"" cardinality=""single"" baseType=""boolean""/>
  <responseProcessing><responseCondition><responseIf>
    <match><variable identifier=""RESPONSE""/><correct identifier=""RESPONSE""/></match>
    <setOutcomeValue identifier=""PASSED""><baseValue baseType=""boolean"">true</baseValue></setOutcomeValue>
  </responseIf></responseCondition></responseProcessing>
</assessmentItem>");

            var outcome = Score(item, "ITM-BOOL").FindElementsByName("outcomeVariable")
                .First(o => o.Identifier() == "PASSED");

            // it used to come back as baseType="string", which is not what the item declared
            Assert.Equal("boolean", outcome.GetAttributeValue("baseType"));
            Assert.Equal("true", outcome.FindElementsByName("value").First().Value);
        }

        [Fact]
        public void BooleanValuesCompareAsBooleansNotText()
        {
            var ctx = TestHelper.GetDefaultResponseProcessingContext(null);
            Assert.True(Helper.CompareSingleValues("true", "TRUE", BaseType.Boolean, ctx));
            Assert.False(Helper.CompareSingleValues("true", "false", BaseType.Boolean, ctx));
        }
    }
}
