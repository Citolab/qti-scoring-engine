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
    public class LteTests
    {
        [Fact]
        public void OneGreaterThanZero()
        {
            // arrange
            var context = TestHelper.GetDefaultResponseProcessingContext(null);
            context.BooleanExpressions = new Dictionary<string, IBooleanExpression>();
            context.BaseValueExpressions = new Dictionary<string, IBaseValueExpression>();
   
            var lte = new Lte();

            context.BooleanExpressions.Add(lte.Name, lte);

            var lteElement = XElement.Parse("<qti-lte></qti-lte>");
            lteElement.Add(1.0F.ToBaseValue().ToXElement());
            lteElement.Add(0.0F.ToBaseValue().ToXElement());
            // act
            var result = lte.Execute(lteElement, context);

            //assert
            Assert.False(result);
        }
        [Fact]
        public void ZeroSmallerThanOne()
        {
            // arrange
            var context = TestHelper.GetDefaultResponseProcessingContext(null);
            context.BooleanExpressions = new Dictionary<string, IBooleanExpression>();
            context.BaseValueExpressions = new Dictionary<string, IBaseValueExpression>();

            var lte = new Lte();

            context.BooleanExpressions.Add(lte.Name, lte);

            var lteElement = XElement.Parse("<qti-lte></qti-lte>");
            lteElement.Add(0.0F.ToBaseValue().ToXElement());
            lteElement.Add(1.0F.ToBaseValue().ToXElement());
            // act
            var result = lte.Execute(lteElement, context);

            //assert
            Assert.True (result);
        }
        [Fact]
        public void EqualReturnsTrue()
        {
            // arrange
            var context = TestHelper.GetDefaultResponseProcessingContext(null);
            context.BooleanExpressions = new Dictionary<string, IBooleanExpression>();
            context.BaseValueExpressions = new Dictionary<string, IBaseValueExpression>();

            var lte = new Lte();

            context.BooleanExpressions.Add(lte.Name, lte);

            var lteElement = XElement.Parse("<qti-lte></qti-lte>");
            lteElement.Add(1.0F.ToBaseValue().ToXElement());
            lteElement.Add(1.0F.ToBaseValue().ToXElement());
            // act
            var result = lte.Execute(lteElement, context);

            //assert
            Assert.True(result);
        }
    }
}
