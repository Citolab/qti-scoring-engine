using Citolab.QTI.ScoringEngine;
using Citolab.QTI.ScoringEngine.Helpers;
using Citolab.QTI.ScoringEngine.Interfaces;
using Citolab.QTI.ScoringEngine.Model;
using Citolab.QTI.ScoringEngine.Tests;
using Microsoft.Extensions.Logging;
using Moq;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Xml.Linq;
using Xunit;

namespace ScoringEngine.Tests.EngineTests
{
    /// <summary>
    /// The AssessmentItem/AssessmentTest and the expression tree built from them are shared by
    /// every result being scored, and by every thread when ProcessParallel is on. These cover
    /// that one result's processing cannot change what another result scores.
    /// </summary>
    public class ConcurrencyAndIsolationTests
    {
        private const string PackageFolder = "Resources/2x/QTI-Packages/RW";
        private static readonly string[] ExpectedScores = { "1", "1", "1", "1" };

        private static List<XDocument> Items() =>
            new DirectoryInfo(Path.Combine(PackageFolder, "Items"))
                .GetFiles("*.xml").OrderBy(f => f.Name)
                .Select(file => TestHelper.GetDocument(file.FullName))
                .ToList();

        /// <summary>One result document per candidate, each tagged with its own sourcedId.</summary>
        private static List<XDocument> Results(int count)
        {
            var source = File.ReadAllText(Path.Combine(PackageFolder, "AssessmentResults", "result1.xml"));
            return Enumerable.Range(0, count).Select(i =>
            {
                var doc = XDocument.Parse(source);
                doc.FindElementByName("context").Attribute("sourcedId").Value = $"candidate-{i:D3}";
                return doc;
            }).ToList();
        }

        private static List<string> ScoresOf(XDocument result, List<XDocument> items) =>
            items.Select(item => result.GetScoreForItem(item.Root.Identifier(), "SCORE")).ToList();

        private static string SourcedIdOf(XDocument result) =>
            result.FindElementByName("context").GetAttributeValue("sourcedId");

        [Fact]
        public void ParallelProcessingKeepsTheOrderItWasGiven()
        {
            var items = Items();
            var results = Results(50);
            var expectedOrder = results.Select(SourcedIdOf).ToList();

            var scored = new Citolab.QTI.ScoringEngine.ScoringEngine().ProcessResponses(new ResponseProcessingContext
            {
                AssessmentItems = items,
                AssessmentmentResults = results,
                Logger = new Mock<ILogger>().Object,
                ProcessParallel = true
            });

            Assert.Equal(expectedOrder, scored.Select(SourcedIdOf).ToList());
        }

        [Fact]
        public void ParallelProcessingScoresTheSameAsSerial()
        {
            var logger = new Mock<ILogger>().Object;

            var serial = new Citolab.QTI.ScoringEngine.ScoringEngine().ProcessResponses(new ResponseProcessingContext
            {
                AssessmentItems = Items(),
                AssessmentmentResults = Results(50),
                Logger = logger,
                ProcessParallel = false
            });
            var parallel = new Citolab.QTI.ScoringEngine.ScoringEngine().ProcessResponses(new ResponseProcessingContext
            {
                AssessmentItems = Items(),
                AssessmentmentResults = Results(50),
                Logger = logger,
                ProcessParallel = true
            });

            var items = Items();
            Assert.Equal(serial.Count, parallel.Count);
            for (var i = 0; i < serial.Count; i++)
            {
                Assert.Equal(ExpectedScores, ScoresOf(serial[i], items));
                Assert.Equal(ScoresOf(serial[i], items), ScoresOf(parallel[i], items));
            }
        }

        [Fact]
        public void EveryResultIsScoredIndependentlyOfTheOnesBeforeIt()
        {
            // the expression tree is reused across results; a value cached or mutated inside it
            // would show up as the later results drifting away from the first.
            var items = Items();
            var scored = new Citolab.QTI.ScoringEngine.ScoringEngine().ProcessResponses(new ResponseProcessingContext
            {
                AssessmentItems = items,
                AssessmentmentResults = Results(25),
                Logger = new Mock<ILogger>().Object
            });

            Assert.All(scored, result => Assert.Equal(ExpectedScores, ScoresOf(result, items)));
        }

        /// <summary>Replaces whatever it is registered for with a value that will never match.</summary>
        private class NeverMatches : ICustomOperator
        {
            public BaseValue Apply(List<BaseValue> values)
            {
                var value = values.FirstOrDefault();
                if (value != null)
                {
                    value.Value = "this-will-never-match";
                }
                return value;
            }
        }

        [Fact]
        public void CustomOperatorsDoNotLeakIntoAnotherEngine()
        {
            var logger = new Mock<ILogger>().Object;
            var item = TestHelper.GetDocument("Resources/2x/ResponseProcessing/CustomOperators.xml");

            AssessmentResult WithResponse()
            {
                var result = TestHelper.GetBasicAssessmentResult();
                result.AddCandidateResponse("ITM-1", "RESPONSE", "tést", BaseType.String, Cardinality.Single);
                return result;
            }

            // engine A overrides a built-in operator so its answer is wrong on purpose
            var overridden = new Citolab.QTI.ScoringEngine.ScoringEngine().ProcessResponses(new ResponseProcessingContext
            {
                AssessmentItems = new List<XDocument> { XDocument.Parse(item.ToString()) },
                AssessmentmentResults = new List<XDocument> { WithResponse() },
                Logger = logger,
                CustomOperators = new Dictionary<string, ICustomOperator> { { "depcp:ToAscii", new NeverMatches() } }
            });
            Assert.Equal("0", overridden[0].GetScoreForItem("ITM-1", "SCORE"));

            // engine B registers nothing, so it must still see the built-in operator
            var builtIn = new Citolab.QTI.ScoringEngine.ScoringEngine().ProcessResponses(new ResponseProcessingContext
            {
                AssessmentItems = new List<XDocument> { XDocument.Parse(item.ToString()) },
                AssessmentmentResults = new List<XDocument> { WithResponse() },
                Logger = logger
            });
            Assert.Equal("1", builtIn[0].GetScoreForItem("ITM-1", "SCORE"));
        }



        [Fact]
        public void OnlyItemsTheCandidateAnsweredAreProcessed()
        {
            // Response processing is driven from the result's itemResults rather than from the
            // full item list; an item with no itemResult must still be left completely alone.
            var items = Items();
            var answered = items[0].Root.Identifier();
            var skipped = items[1].Root.Identifier();

            var result = Results(1).Single();
            result.RemoveItemResult(skipped);

            var scored = new Citolab.QTI.ScoringEngine.ScoringEngine().ProcessResponses(new ResponseProcessingContext
            {
                AssessmentItems = items,
                AssessmentmentResults = new List<XDocument> { result },
                Logger = new Mock<ILogger>().Object
            });

            Assert.Equal("1", scored[0].GetScoreForItem(answered, "SCORE"));
            Assert.Null(scored[0].GetScoreForItem(skipped, "SCORE"));
            // and no empty itemResult was invented for it
            Assert.DoesNotContain(scored[0].FindElementsByName("itemResult"), e => e.Identifier() == skipped);
        }

        [Fact]
        public void DuplicateItemIdentifiersAreReportedAndTheFirstIsUsed()
        {
            var items = Items();
            var duplicated = items.Concat(new List<XDocument> { XDocument.Parse(items[0].ToString()) }).ToList();
            var log = new Mock<ILogger>();

            var scored = new Citolab.QTI.ScoringEngine.ScoringEngine().ProcessResponses(new ResponseProcessingContext
            {
                AssessmentItems = duplicated,
                AssessmentmentResults = Results(1),
                Logger = log.Object
            });

            Assert.Equal(ExpectedScores, ScoresOf(scored[0], items));
            log.Verify(l => l.Log(LogLevel.Warning, It.IsAny<EventId>(), It.IsAny<It.IsAnyType>(),
                It.IsAny<System.Exception>(), It.IsAny<System.Func<It.IsAnyType, System.Exception, string>>()), Times.AtLeastOnce);
        }

        /// <summary>
        /// Sets an outcome that has no outcomeDeclaration. ResetOutcomes then has to add the
        /// declaration, which is the write that used to land in the shared AssessmentItem.
        /// </summary>
        private const string ItemSettingAnUndeclaredOutcome = @"<assessmentItem xmlns=""http://www.imsglobal.org/xsd/imsqti_v2p2"" identifier=""ITM-RACE"" title=""race"" timeDependent=""false"">
  <responseDeclaration identifier=""RESPONSE"" cardinality=""single"" baseType=""string"">
    <correctResponse><value>test</value></correctResponse>
  </responseDeclaration>
  <outcomeDeclaration identifier=""SCORE"" cardinality=""single"" baseType=""float"">
    <defaultValue><value>0</value></defaultValue>
  </outcomeDeclaration>
  <responseProcessing>
    <responseCondition>
      <responseIf>
        <match><variable identifier=""RESPONSE""/><correct identifier=""RESPONSE""/></match>
        <setOutcomeValue identifier=""SCORE""><baseValue baseType=""float"">1</baseValue></setOutcomeValue>
        <setOutcomeValue identifier=""UNDECLARED""><baseValue baseType=""float"">7</baseValue></setOutcomeValue>
      </responseIf>
    </responseCondition>
  </responseProcessing>
</assessmentItem>";

        [Fact]
        public void AnUndeclaredOutcomeIsAddedPerResultNotToTheSharedItem()
        {
            // The AssessmentItem is built once and shared by every result and every thread.
            // Adding the missing declaration to it meant concurrent writes to one Dictionary.
            var results = Enumerable.Range(0, 200).Select(i =>
            {
                var result = TestHelper.GetBasicAssessmentResult();
                result.AddCandidateResponse("ITM-RACE", "RESPONSE", "test", BaseType.String, Cardinality.Single);
                return (XDocument)result;
            }).ToList();

            var scored = new Citolab.QTI.ScoringEngine.ScoringEngine().ProcessResponses(new ResponseProcessingContext
            {
                AssessmentItems = new List<XDocument> { XDocument.Parse(ItemSettingAnUndeclaredOutcome) },
                AssessmentmentResults = results,
                Logger = new Mock<ILogger>().Object,
                ProcessParallel = true
            });

            Assert.All(scored, result =>
            {
                Assert.Equal("1", result.GetScoreForItem("ITM-RACE", "SCORE"));
                Assert.Equal("7", result.GetScoreForItem("ITM-RACE", "UNDECLARED"));
            });
        }

        [Fact]
        public void CustomOperatorsPassedOnALaterCallAreNotIgnored()
        {
            var logger = new Mock<ILogger>().Object;
            var item = TestHelper.GetDocument("Resources/2x/ResponseProcessing/CustomOperators.xml");
            var engine = new Citolab.QTI.ScoringEngine.ScoringEngine();

            List<XDocument> Run(Dictionary<string, ICustomOperator> operators)
            {
                var result = TestHelper.GetBasicAssessmentResult();
                result.AddCandidateResponse("ITM-1", "RESPONSE", "tést", BaseType.String, Cardinality.Single);
                return engine.ProcessResponses(new ResponseProcessingContext
                {
                    AssessmentItems = new List<XDocument> { XDocument.Parse(item.ToString()) },
                    AssessmentmentResults = new List<XDocument> { result },
                    Logger = logger,
                    CustomOperators = operators
                });
            }

            // the same engine instance, first without an override and then with one
            Assert.Equal("1", Run(new Dictionary<string, ICustomOperator>())[0].GetScoreForItem("ITM-1", "SCORE"));
            Assert.Equal("0", Run(new Dictionary<string, ICustomOperator> { { "depcp:ToAscii", new NeverMatches() } })[0].GetScoreForItem("ITM-1", "SCORE"));
        }
    }
}
