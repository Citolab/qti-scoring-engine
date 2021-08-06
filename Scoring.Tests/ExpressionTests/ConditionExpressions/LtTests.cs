using System;
using Citolab.QTI.ScoringEngine.Helpers;
using Citolab.QTI.ScoringEngine.Expressions.ConditionExpressions;
using Citolab.QTI.ScoringEngine.Interfaces;
using Citolab.QTI.ScoringEngine.Tests;
using System.Collections.Generic;
using System.Xml.Linq;
using Xunit;

namespace ScoringEngine.Tests.ExpressionsTests.ConditionExpressions
{
    public class LtTests
    {
        [Fact]
        public void OneGreaterThanZero()
        {
            // arrange
            var context = TestHelper.GetDefaultResponseProcessingContext(null);
            var lt = new Lt();

            var ltElement = XElement.Parse("<qti-lt></qti-lt>");
            ltElement.Add(1.0F.ToBaseValue().ToXElement());
            ltElement.Add(0.0F.ToBaseValue().ToXElement());
            // act
            lt.Init(ltElement, TestHelper.GetExpressionFactory());
            var result = lt.Execute(context);

            //assert
            Assert.False(result);
        }
        [Fact]
        public void ZeroSmallerThanOne()
        {
            // arrange
            var context = TestHelper.GetDefaultResponseProcessingContext(null);

            var lt = new Lt();
            var ltElement = XElement.Parse("<qti-lt></qti-lt>");
            ltElement.Add(0.0F.ToBaseValue().ToXElement());
            ltElement.Add(1.0F.ToBaseValue().ToXElement());
            // act
            lt.Init(ltElement, TestHelper.GetExpressionFactory());
            var result = lt.Execute(context);

            //assert
            Assert.True (result);
        }
        [Fact]
        public void EqualReturnsFalse()
        {
            // arrange
            var context = TestHelper.GetDefaultResponseProcessingContext(null);

            var lt = new Lt();

            var ltElement = XElement.Parse("<qti-lt></qti-lt>");
            ltElement.Add(1.0F.ToBaseValue().ToXElement());
            ltElement.Add(1.0F.ToBaseValue().ToXElement());
            // act
            lt.Init(ltElement, TestHelper.GetExpressionFactory());
            var result = lt.Execute(context);

            //assert
            Assert.False(result);
        }
    }
}
