using System;
using Citolab.QTI.Scoring.Helper;
using Citolab.QTI.Scoring.ResponseProcessing.Operators;
using Citolab.QTI.Scoring.ResponseProcessing.Interfaces;
using Citolab.QTI.Scoring.Tests;
using System.Collections.Generic;
using System.Xml.Linq;
using Xunit;

namespace ScoringEngine.Tests.ResponseProcessingTests.Operators
{
    public class GteTests
    {
        [Fact]
        public void OneGreaterThanZero()
        {
            // arrange
            var context = TestHelper.GetDefaultResponseProcessingContext(null);
            context.Operators = new Dictionary<string, IResponseProcessingOperator>();
            context.Expressions = new Dictionary<string, IResponseProcessingExpression>();
   
            var gte = new Gte();

            context.Operators.Add(gte.Name, gte);

            var gteElement = XElement.Parse("<gte></gte>");
            gteElement.Add(1.0F.ToBaseValue().ToXElement());
            gteElement.Add(0.0F.ToBaseValue().ToXElement());
            // act
            var result = gte.Execute(gteElement, context);

            //assert
            Assert.True(result);
        }
        [Fact]
        public void ZeroSmallerThanOne()
        {
            // arrange
            var context = TestHelper.GetDefaultResponseProcessingContext(null);
            context.Operators = new Dictionary<string, IResponseProcessingOperator>();
            context.Expressions = new Dictionary<string, IResponseProcessingExpression>();

            var gte = new Gte();

            context.Operators.Add(gte.Name, gte);

            var gteElement = XElement.Parse("<gte></gte>");
            gteElement.Add(0.0F.ToBaseValue().ToXElement());
            gteElement.Add(1.0F.ToBaseValue().ToXElement());
            // act
            var result = gte.Execute(gteElement, context);

            //assert
            Assert.False (result);
        }
        [Fact]
        public void EqualReturnsTrue()
        {
            // arrange
            var context = TestHelper.GetDefaultResponseProcessingContext(null);
            context.Operators = new Dictionary<string, IResponseProcessingOperator>();
            context.Expressions = new Dictionary<string, IResponseProcessingExpression>();

            var gte = new Gte();

            context.Operators.Add(gte.Name, gte);

            var gteElement = XElement.Parse("<gte></gte>");
            gteElement.Add(1.0F.ToBaseValue().ToXElement());
            gteElement.Add(1.0F.ToBaseValue().ToXElement());
            // act
            var result = gte.Execute(gteElement, context);

            //assert
            Assert.True(result);
        }
    }
}
