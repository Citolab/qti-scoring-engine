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
        public AndTests()
        {
            ReturnTrue.Init();
            ReturnFalse.Init();
        }
        [Fact]
        public void CheckTwoTrueValues()
        {
            // arrange
            var context = TestHelper.GetDefaultResponseProcessingContext(null);
            var and = new And();
            var andElement = XElement.Parse($"<qti-and></qti-and>");
            andElement.Add(XElement.Parse("<returnTrue/>"));
            andElement.Add(XElement.Parse("<returnTrue/>"));

            // act
            and.Init(andElement, TestHelper.GetExpressionFactory());
            var result = and.Execute(context);

            //assert
            Assert.True(result);
        }

        [Fact]
        public void CheckOneTrueAndFalseValues()
        {
            // arrange
            var context = TestHelper.GetDefaultResponseProcessingContext(null);

            var and = new And();

            var andElement = XElement.Parse($"<qti-and></qti-and>");
            andElement.Add(XElement.Parse("<returnTrue/>"));
            andElement.Add(XElement.Parse("<returnFalse/>"));

            and.Init(andElement, TestHelper.GetExpressionFactory());
            // act
            var result = and.Execute(context);
            //assert
            Assert.False(result);
        }

        [Fact]
        public void CheckFalseValues()
        {
            // arrange
            var context = TestHelper.GetDefaultResponseProcessingContext(null);

            var and = new And();
            var andElement = XElement.Parse($"<qti-and></qti-and>");
            andElement.Add(XElement.Parse("<returnFalse/>"));
            andElement.Add(XElement.Parse("<returnFalse/>"));
            // act
            and.Init(andElement, TestHelper.GetExpressionFactory());
            var result = and.Execute(context);
            //assert
            Assert.False(result);
        }
    }
}
