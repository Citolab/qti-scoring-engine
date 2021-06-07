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
    public class EqualRoundedTests
    {
        // In the QTI docs: For example, if significantFigures mode is used with figures=3
        // and the values are 3.175 and 3.183, the result is true. 
        // I assumed it's an error in the documentation and figures=2 is meant
        [Fact]
        public void EqualRounded_True()
        {
            // arrange
            var context = TestHelper.GetDefaultResponseProcessingContext(null);
            context.ConditionExpressions = new Dictionary<string, IConditionExpression>();
   
            var equalRounded = new EqualRounded();

            context.ConditionExpressions.Add(equalRounded.Name, equalRounded);

            var equalRoundedElement = XElement.Parse(@"<qti-equal-rounded rounding-mode=""significantFigures"" figures=""2"" ></qti-equal-rounded>");
            equalRoundedElement.Add(3.175F.ToBaseValue().ToXElement());
            equalRoundedElement.Add(3.183F.ToBaseValue().ToXElement());
            // act
            var result = equalRounded.Execute(equalRoundedElement, context);

            //assert
            Assert.True(result);
        }

        [Fact]
        public void EqualRounded_True_Default()
        {
            // arrange
            var context = TestHelper.GetDefaultResponseProcessingContext(null);
            context.ConditionExpressions = new Dictionary<string, IConditionExpression>();

            var equalRounded = new EqualRounded();

            context.ConditionExpressions.Add(equalRounded.Name, equalRounded);

            var equalRoundedElement = XElement.Parse(@"<qti-equal-rounded figures=""2"" ></qti-equal-rounded>");
            equalRoundedElement.Add(3.175F.ToBaseValue().ToXElement());
            equalRoundedElement.Add(3.183F.ToBaseValue().ToXElement());
            // act
            var result = equalRounded.Execute(equalRoundedElement, context);

            //assert
            Assert.True(result);
        }

        [Fact]
        public void EqualRounded_False()
        {
            // arrange
            var context = TestHelper.GetDefaultResponseProcessingContext(null);
            context.ConditionExpressions = new Dictionary<string, IConditionExpression>();

            var equalRounded = new EqualRounded();

            context.ConditionExpressions.Add(equalRounded.Name, equalRounded);

            var equalRoundedElement = XElement.Parse(@"<qti-equal-rounded rounding-mode=""significantFigures"" figures=""3"" ></qti-equal-rounded>");
            equalRoundedElement.Add(3.175F.ToBaseValue().ToXElement());
            equalRoundedElement.Add(3.1749F.ToBaseValue().ToXElement());
            // act
            var result = equalRounded.Execute(equalRoundedElement, context);

            //assert
            Assert.True(result);
        }

        [Fact]
        public void EqualRounded_DecimalPlaces_True()
        {
            // arrange
            var context = TestHelper.GetDefaultResponseProcessingContext(null);
            context.ConditionExpressions = new Dictionary<string, IConditionExpression>();

            var equalRounded = new EqualRounded();

            context.ConditionExpressions.Add(equalRounded.Name, equalRounded);

            var equalRoundedElement = XElement.Parse(@"<qti-equal-rounded rounding-mode=""decimalPlaces"" figures=""2"" ></qti-equal-rounded>");
            equalRoundedElement.Add(1.68572F.ToBaseValue().ToXElement());
            equalRoundedElement.Add(1.69F.ToBaseValue().ToXElement());
            // act
            var result = equalRounded.Execute(equalRoundedElement, context);

            //assert
            Assert.True(result);
        }

        [Fact]
        public void EqualRounded_DecimalPlaces_False()
        {
            // arrange
            var context = TestHelper.GetDefaultResponseProcessingContext(null);
            context.ConditionExpressions = new Dictionary<string, IConditionExpression>();

            var equalRounded = new EqualRounded();

            context.ConditionExpressions.Add(equalRounded.Name, equalRounded);

            var equalRoundedElement = XElement.Parse(@"<qti-equal-rounded rounding-mode=""decimalPlaces"" figures=""2"" ></qti-equal-rounded>");
            equalRoundedElement.Add(1.68572F.ToBaseValue().ToXElement());
            equalRoundedElement.Add(1.68432F.ToBaseValue().ToXElement());
            // act
            var result = equalRounded.Execute(equalRoundedElement, context);
 
            //assert
            Assert.False(result);
        }

    }
}
