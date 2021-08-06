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
    public class GtTests
    {
        [Fact]
        public void OneGreaterThanZero()
        {
            // arrange
            var context = TestHelper.GetDefaultResponseProcessingContext(null);

            var gt = new Gt();

            var gtElement = XElement.Parse("<qti-gt></qti-gt>");
            gtElement.Add(1.0F.ToBaseValue().ToXElement());
            gtElement.Add(0.0F.ToBaseValue().ToXElement());
            // act
            gt.Init(gtElement, TestHelper.GetExpressionFactory());
            var result = gt.Execute(context);

            //assert
            Assert.True(result);
        }
        [Fact]
        public void ZeroSmallerThanOne()
        {
            // arrange
            var context = TestHelper.GetDefaultResponseProcessingContext(null);

            var gt = new Gt();
            var gtElement = XElement.Parse("<qti-gt></qti-gt>");
            gtElement.Add(0.0F.ToBaseValue().ToXElement());
            gtElement.Add(1.0F.ToBaseValue().ToXElement());
            // act
            gt.Init(gtElement, TestHelper.GetExpressionFactory());
            var result = gt.Execute(context);

            //assert
            Assert.False (result);
        }
        [Fact]
        public void EqualReturnsFalse()
        {
            // arrange
            var context = TestHelper.GetDefaultResponseProcessingContext(null);

            var gt = new Gt();
            var gtElement = XElement.Parse("<qti-gt></qti-gt>");
            gtElement.Add(1.0F.ToBaseValue().ToXElement());
            gtElement.Add(1.0F.ToBaseValue().ToXElement());
            // act
            gt.Init(gtElement, TestHelper.GetExpressionFactory());
            var result = gt.Execute(context);

            //assert
            Assert.False(result);
        }
    }
}
