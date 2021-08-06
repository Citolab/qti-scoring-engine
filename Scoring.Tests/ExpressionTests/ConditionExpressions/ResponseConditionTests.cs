using Citolab.QTI.ScoringEngine.Helpers;
using Citolab.QTI.ScoringEngine.ResponseProcessing;
using Citolab.QTI.ScoringEngine.Expressions.ConditionExpressions;
using Citolab.QTI.ScoringEngine.Tests;
using Citolab.QTI.ScoringEngine.Model;
using Moq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;
using Xunit;
using Microsoft.Extensions.Logging;

namespace ScoringEngine.Tests.ExpressionsTests.ConditionExpressions
{
    public class ResponseConditionTests
    {

        [Fact]
        public void ExecuteIf()
        {
            // arrange
            var logger = new Mock<ILogger>().Object;
            var context = TestHelper.GetDefaultResponseProcessingContext(new AssessmentItem(logger, XDocument.Parse("<qti-asssessment-item/>"), TestHelper.GetExpressionFactory()));

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
            context.OutcomeDeclarations = context.AssessmentItem.OutcomeDeclarations;

            var responseConditionElement = XElement.Parse("<qti-response-condition></qti-response-condition>");
            responseConditionElement.Add(new ResponseIf().GetElementWithValue(true, "qti-if", "UNITTEST_RESULT"));
            responseConditionElement.Add(new ResponseElseIf().GetElementWithValue(true, "qti-else-if", "UNITTEST_RESULT"));
            responseConditionElement.Add(new ResponseElse().GetElementWithValue(true, "qti-else", "UNITTEST_RESULT"));
            // act
            var responseCondition = new ResponseCondition();
            responseCondition.Init(responseConditionElement, TestHelper.GetExpressionFactory());
            responseCondition.Execute(context);

            //assert
            Assert.Equal("qti-if", context.ItemResult.OutcomeVariables["UNITTEST_RESULT"].Value);
        }

        [Fact]
        public void ExecuteElseIf()
        {
            // arrange
            var logger = new Mock<ILogger>().Object;
            var context = TestHelper.GetDefaultResponseProcessingContext(new AssessmentItem(logger, XDocument.Parse("<qti-asssessment-item/>"), TestHelper.GetExpressionFactory()));

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
            context.OutcomeDeclarations = context.AssessmentItem.OutcomeDeclarations;

            var responseConditionElement = XElement.Parse("<qti-response-condition></qti-response-condition>");
            responseConditionElement.Add(new ResponseIf().GetElementWithValue(false, "qti-if", "UNITTEST_RESULT"));
            responseConditionElement.Add(new ResponseElseIf().GetElementWithValue(true, "qti-else-if", "UNITTEST_RESULT"));
            responseConditionElement.Add(new ResponseElse().GetElementWithValue(true, "qti-else", "UNITTEST_RESULT"));
            // act
            var responseCondition = new ResponseCondition();
            responseCondition.Init(responseConditionElement, TestHelper.GetExpressionFactory());
            responseCondition.Execute(context);

            //assert
            Assert.Equal("qti-else-if", context.ItemResult.OutcomeVariables["UNITTEST_RESULT"].Value);
        }

        [Fact]
        public void ExecuteElse()
        {
            // arrange
            var logger = new Mock<ILogger>().Object;
            var context = TestHelper.GetDefaultResponseProcessingContext(new AssessmentItem(logger, XDocument.Parse("<asssessmentItem/>"), TestHelper.GetExpressionFactory()));

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
            context.OutcomeDeclarations = context.AssessmentItem.OutcomeDeclarations;

            var responseConditionElement = XElement.Parse("<qti-response-condition></qti-response-condition>");
            responseConditionElement.Add(new ResponseIf().GetElementWithValue(false, "qti-if", "UNITTEST_RESULT"));
            responseConditionElement.Add(new ResponseElseIf().GetElementWithValue(false, "qti-else-if", "UNITTEST_RESULT"));
            responseConditionElement.Add(new ResponseElse().GetElementWithValue(true, "qti-else", "UNITTEST_RESULT"));
            // act
            var responseCondition = new ResponseCondition();
            responseCondition.Init(responseConditionElement, TestHelper.GetExpressionFactory());
            responseCondition.Execute(context);

            //assert
            Assert.Equal("qti-else", context.ItemResult.OutcomeVariables["UNITTEST_RESULT"].Value);
        }
    }
}
