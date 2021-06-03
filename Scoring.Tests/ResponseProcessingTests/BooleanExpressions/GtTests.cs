using System;
using Citolab.QTI.ScoringEngine.Helper;
using Citolab.QTI.ScoringEngine.ResponseProcessing.BooleanExpressions;
using Citolab.QTI.ScoringEngine.ResponseProcessing.Interfaces;
using Citolab.QTI.ScoringEngine.Tests;
using System.Collections.Generic;
using System.Xml.Linq;
using Xunit;

namespace ScoringEngine.Tests.ResponseProcessingTests.BooleanExpressions
{
    public class GtTests
    {
        [Fact]
        public void OneGreaterThanZero()
        {
            // arrange
            var context = TestHelper.GetDefaultResponseProcessingContext(null);
            context.BooleanExpressions = new Dictionary<string, IBooleanExpression>();
            context.BaseValueExpressions = new Dictionary<string, IBaseValueExpression>();
   
            var gt = new Gt();

            context.BooleanExpressions.Add(gt.Name, gt);

            var gtElement = XElement.Parse("<qti-gt></qti-gt>");
            gtElement.Add(1.0F.ToBaseValue().ToXElement());
            gtElement.Add(0.0F.ToBaseValue().ToXElement());
            // act
            var result = gt.Execute(gtElement, context);

            //assert
            Assert.True(result);
        }
        [Fact]
        public void ZeroSmallerThanOne()
        {
            // arrange
            var context = TestHelper.GetDefaultResponseProcessingContext(null);
            context.BooleanExpressions = new Dictionary<string, IBooleanExpression>();
            context.BaseValueExpressions = new Dictionary<string, IBaseValueExpression>();

            var gt = new Gt();

            context.BooleanExpressions.Add(gt.Name, gt);

            var gtElement = XElement.Parse("<qti-gt></qti-gt>");
            gtElement.Add(0.0F.ToBaseValue().ToXElement());
            gtElement.Add(1.0F.ToBaseValue().ToXElement());
            // act
            var result = gt.Execute(gtElement, context);

            //assert
            Assert.False (result);
        }
        [Fact]
        public void EqualReturnsFalse()
        {
            // arrange
            var context = TestHelper.GetDefaultResponseProcessingContext(null);
            context.BooleanExpressions = new Dictionary<string, IBooleanExpression>();
            context.BaseValueExpressions = new Dictionary<string, IBaseValueExpression>();

            var gt = new Gt();

            context.BooleanExpressions.Add(gt.Name, gt);

            var gtElement = XElement.Parse("<qti-gt></qti-gt>");
            gtElement.Add(1.0F.ToBaseValue().ToXElement());
            gtElement.Add(1.0F.ToBaseValue().ToXElement());
            // act
            var result = gt.Execute(gtElement, context);

            //assert
            Assert.False(result);
        }
    }
}
