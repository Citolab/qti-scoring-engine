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
    public class LteTests
    {
        [Fact]
        public void OneGreaterThanZero()
        {
            // arrange
            var context = TestHelper.GetDefaultResponseProcessingContext(null);
            var lte = new Lte();

            var lteElement = XElement.Parse("<qti-lte></qti-lte>");
            lteElement.Add(1.0F.ToBaseValue().ToXElement());
            lteElement.Add(0.0F.ToBaseValue().ToXElement());
            // act
            lte.Init(lteElement, TestHelper.GetExpressionFactory());
            var result = lte.Execute(context);

            //assert
            Assert.False(result);
        }
        [Fact]
        public void ZeroSmallerThanOne()
        {
            // arrange
            var context = TestHelper.GetDefaultResponseProcessingContext(null);

            var lte = new Lte();

            var lteElement = XElement.Parse("<qti-lte></qti-lte>");
            lteElement.Add(0.0F.ToBaseValue().ToXElement());
            lteElement.Add(1.0F.ToBaseValue().ToXElement());
            // act
            lte.Init(lteElement, TestHelper.GetExpressionFactory());
            var result = lte.Execute(context);

            //assert
            Assert.True (result);
        }
        [Fact]
        public void EqualReturnsTrue()
        {
            // arrange
            var context = TestHelper.GetDefaultResponseProcessingContext(null);

            var lte = new Lte();

            var lteElement = XElement.Parse("<qti-lte></qti-lte>");
            lteElement.Add(1.0F.ToBaseValue().ToXElement());
            lteElement.Add(1.0F.ToBaseValue().ToXElement());
            // act
            lte.Init(lteElement, TestHelper.GetExpressionFactory());
            var result = lte.Execute(context);

            //assert
            Assert.True(result);
        }
    }
}
