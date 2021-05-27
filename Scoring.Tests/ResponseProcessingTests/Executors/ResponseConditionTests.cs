using Citolab.QTI.Scoring.Helper;
using Citolab.QTI.Scoring.ResponseProcessing;
using Citolab.QTI.Scoring.ResponseProcessing.Executors;
using Citolab.QTI.Scoring.Tests;
using Citolab.QTI.Scoring.Model;
using Moq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;
using Xunit;
using Microsoft.Extensions.Logging;

namespace ScoringEngine.Tests.ResponseProcessingTests.Executors
{
    public class ResponseConditionTests
    {

        [Fact]
        public void ExecuteIf()
        {
            // arrange
            var logger = new Mock<ILogger>().Object;
            var context = TestHelper.GetDefaultResponseProcessingContext(new AssessmentItem(logger, XDocument.Parse("<asssessmentItem/>")));

            context.AssessmentItem.OutcomeDeclarations = new Dictionary<string, OutcomeDeclaration>
            {
                {
                    "UNITTEST_RESULT",
                    new OutcomeDeclaration
                    {
                        BaseType = BaseType.String,
                        Cardinality = Cardinality.Single,
                        Identifier = "UNITTEST_RESULT",
                        DefaultValue = 0
                    }
                }
            };

            var responseConditionElement = XElement.Parse("<responseCondition></responseCondition>");
            responseConditionElement.Add(new ResponseIf().GetElementWithValue(true, "if", "UNITTEST_RESULT"));
            responseConditionElement.Add(new ResponseElseIf().GetElementWithValue(true, "elseIf", "UNITTEST_RESULT"));
            responseConditionElement.Add(new ResponseElse().GetElementWithValue(true, "else", "UNITTEST_RESULT"));
            // act
            new ResponseCondition().Execute(responseConditionElement, context);

            //assert
            Assert.Equal("if", context.ItemResult.OutcomeVariables["UNITTEST_RESULT"].Value);
        }

        [Fact]
        public void ExecuteElseIf()
        {
            // arrange
            var logger = new Mock<ILogger>().Object;
            var context = TestHelper.GetDefaultResponseProcessingContext(new AssessmentItem(logger, XDocument.Parse("<asssessmentItem/>")));

            context.AssessmentItem.OutcomeDeclarations = new Dictionary<string, OutcomeDeclaration>
            {
                {
                    "UNITTEST_RESULT",
                    new OutcomeDeclaration
                    {
                        BaseType = BaseType.String,
                        Cardinality = Cardinality.Single,
                        Identifier = "UNITTEST_RESULT",
                        DefaultValue = 0
                    }
                }
            };

            var responseConditionElement = XElement.Parse("<responseCondition></responseCondition>");
            responseConditionElement.Add(new ResponseIf().GetElementWithValue(false, "if", "UNITTEST_RESULT"));
            responseConditionElement.Add(new ResponseElseIf().GetElementWithValue(true, "elseIf", "UNITTEST_RESULT"));
            responseConditionElement.Add(new ResponseElse().GetElementWithValue(true, "else", "UNITTEST_RESULT"));
            // act
            new ResponseCondition().Execute(responseConditionElement, context);

            //assert
            Assert.Equal("elseIf", context.ItemResult.OutcomeVariables["UNITTEST_RESULT"].Value);
        }

        [Fact]
        public void ExecuteElse()
        {
            // arrange
            var logger = new Mock<ILogger>().Object;
            var context = TestHelper.GetDefaultResponseProcessingContext(new AssessmentItem(logger, XDocument.Parse("<asssessmentItem/>")));

            context.AssessmentItem.OutcomeDeclarations = new Dictionary<string, OutcomeDeclaration>
            {
                {
                    "UNITTEST_RESULT",
                    new OutcomeDeclaration
                    {
                        BaseType = BaseType.String,
                        Cardinality = Cardinality.Single,
                        Identifier = "UNITTEST_RESULT",
                        DefaultValue = 0
                    }
                }
            };

            var responseConditionElement = XElement.Parse("<responseCondition></responseCondition>");
            responseConditionElement.Add(new ResponseIf().GetElementWithValue(false, "if", "UNITTEST_RESULT"));
            responseConditionElement.Add(new ResponseElseIf().GetElementWithValue(false, "elseIf", "UNITTEST_RESULT"));
            responseConditionElement.Add(new ResponseElse().GetElementWithValue(true, "else", "UNITTEST_RESULT"));
            // act
            new ResponseCondition().Execute(responseConditionElement, context);

            //assert
            Assert.Equal("else", context.ItemResult.OutcomeVariables["UNITTEST_RESULT"].Value);
        }
    }
}
