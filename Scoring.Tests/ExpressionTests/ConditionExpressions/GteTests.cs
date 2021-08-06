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
    public class GteTests
    {
        [Fact]
        public void OneGreaterThanZero()
        {
            // arrange
            var context = TestHelper.GetDefaultResponseProcessingContext(null);
            var gte = new Gte();

            var gteElement = XElement.Parse("<qti-gte></qti-gte>");
            gteElement.Add(1.0F.ToBaseValue().ToXElement());
            gteElement.Add(0.0F.ToBaseValue().ToXElement());
            // act
            gte.Init(gteElement, TestHelper.GetExpressionFactory());
            var result = gte.Execute(context);
            //assert
            Assert.True(result);
        }
        [Fact]
        public void ZeroSmallerThanOne()
        {
            // arrange
            var context = TestHelper.GetDefaultResponseProcessingContext(null);

            var gte = new Gte();

            var gteElement = XElement.Parse("<qti-gte></qti-gte>");
            gteElement.Add(0.0F.ToBaseValue().ToXElement());
            gteElement.Add(1.0F.ToBaseValue().ToXElement());
            // act
            gte.Init(gteElement, TestHelper.GetExpressionFactory());
            var result = gte.Execute(context);

            //assert
            Assert.False (result);
        }
        [Fact]
        public void EqualReturnsTrue()
        {
            // arrange
            var context = TestHelper.GetDefaultResponseProcessingContext(null);

            var gte = new Gte();

            var gteElement = XElement.Parse("<qti-gte></qti-gte>");
            gteElement.Add(1.0F.ToBaseValue().ToXElement());
            gteElement.Add(1.0F.ToBaseValue().ToXElement());
            // act
            gte.Init(gteElement, TestHelper.GetExpressionFactory());
            var result = gte.Execute(context);

            //assert
            Assert.True(result);
        }
    }
}
