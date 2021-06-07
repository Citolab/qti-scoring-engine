using Citolab.QTI.ScoringEngine.Helpers;
using Citolab.QTI.ScoringEngine.ResponseProcessing;
using Citolab.QTI.ScoringEngine.Expressions.ConditionExpressions;
using Citolab.QTI.ScoringEngine.Tests;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;
using Xunit;
using Citolab.QTI.ScoringEngine.Interfaces;

namespace ScoringEngine.Tests.ExpressionsTests.ConditionExpressions
{
    public class AndTests
    {
        [Fact]
        public void CheckTwoTrueValues()
        {
            // arrange
            var context = TestHelper.GetDefaultResponseProcessingContext(null);
            context.ConditionExpressions = new Dictionary<string, IConditionExpression>();

            var returnTrue = new ReturnTrue();
            var and = new And();

            context.ConditionExpressions.Add(and.Name, and);
            context.ConditionExpressions.Add(returnTrue.Name, returnTrue);

            var andElement = XElement.Parse($"<{and.Name}></{and.Name}>");
            andElement.Add(XElement.Parse("<returnTrue/>"));
            andElement.Add(XElement.Parse("<returnTrue/>"));

            
            // act
            var result = and.Execute(andElement, context);

            //assert
            Assert.True(result);
        }

        [Fact]
        public void CheckOneTrueAndFalseValues()
        {
            // arrange
            var context = TestHelper.GetDefaultResponseProcessingContext(null);
            context.ConditionExpressions = new Dictionary<string, IConditionExpression>();
            var returnTrue = new ReturnTrue();
            var returnFalse = new ReturnFalse();
            var and = new And();
            context.ConditionExpressions.Add(and.Name, and);
            context.ConditionExpressions.Add(returnTrue.Name, returnTrue);
            context.ConditionExpressions.Add(returnFalse.Name, returnFalse);
            var andElement = XElement.Parse($"<{and.Name}></{and.Name}>");
            andElement.Add(XElement.Parse("<returnTrue/>"));
            andElement.Add(XElement.Parse("<returnFalse/>"));

            // act
            var result = and.Execute(andElement, context);
            //assert
            Assert.False(result);
        }

        [Fact]
        public void CheckFalseValues()
        {
            // arrange
            var context = TestHelper.GetDefaultResponseProcessingContext(null);
            context.ConditionExpressions = new Dictionary<string, IConditionExpression>();
            var returnTrue = new ReturnTrue();
            var returnFalse = new ReturnFalse();

            var and = new And();
            context.ConditionExpressions.Add(and.Name, and);
            context.ConditionExpressions.Add(returnTrue.Name, returnTrue);
            context.ConditionExpressions.Add(returnFalse.Name, returnFalse);
            var andElement = XElement.Parse($"<qti-and></qti-and>");
            andElement.Add(XElement.Parse("<returnFalse/>"));
            andElement.Add(XElement.Parse("<returnFalse/>"));
            // act
            var result = and.Execute(andElement, context);
            //assert
            Assert.False(result);
        }
    }
}
