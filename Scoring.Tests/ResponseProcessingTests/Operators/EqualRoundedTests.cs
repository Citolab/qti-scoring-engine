using System;
using Citolab.QTI.ScoringEngine.Helper;
using Citolab.QTI.ScoringEngine.ResponseProcessing.Operators;
using Citolab.QTI.ScoringEngine.ResponseProcessing.Interfaces;
using Citolab.QTI.ScoringEngine.Tests;
using System.Collections.Generic;
using System.Xml.Linq;
using Xunit;

namespace ScoringEngine.Tests.ResponseProcessingTests.Operators
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
            context.Operators = new Dictionary<string, IResponseProcessingOperator>();
            context.Expressions = new Dictionary<string, IResponseProcessingExpression>();
   
            var equalRounded = new EqualRounded();

            context.Operators.Add(equalRounded.Name, equalRounded);

            var equalRoundedElement = XElement.Parse(@"<equalRounded roundingMode=""significantFigures"" figures=""2"" ></equalRounded>");
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
            context.Operators = new Dictionary<string, IResponseProcessingOperator>();
            context.Expressions = new Dictionary<string, IResponseProcessingExpression>();

            var equalRounded = new EqualRounded();

            context.Operators.Add(equalRounded.Name, equalRounded);

            var equalRoundedElement = XElement.Parse(@"<equalRounded figures=""2"" ></equalRounded>");
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
            context.Operators = new Dictionary<string, IResponseProcessingOperator>();
            context.Expressions = new Dictionary<string, IResponseProcessingExpression>();

            var equalRounded = new EqualRounded();

            context.Operators.Add(equalRounded.Name, equalRounded);

            var equalRoundedElement = XElement.Parse(@"<equalRounded roundingMode=""significantFigures"" figures=""3"" ></equalRounded>");
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
            context.Operators = new Dictionary<string, IResponseProcessingOperator>();
            context.Expressions = new Dictionary<string, IResponseProcessingExpression>();

            var equalRounded = new EqualRounded();

            context.Operators.Add(equalRounded.Name, equalRounded);

            var equalRoundedElement = XElement.Parse(@"<equalRounded roundingMode=""significantFigures"" figures=""2"" ></equalRounded>");
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
            context.Operators = new Dictionary<string, IResponseProcessingOperator>();
            context.Expressions = new Dictionary<string, IResponseProcessingExpression>();

            var equalRounded = new EqualRounded();

            context.Operators.Add(equalRounded.Name, equalRounded);

            var equalRoundedElement = XElement.Parse(@"<equalRounded roundingMode=""significantFigures"" figures=""2"" ></equalRounded>");
            equalRoundedElement.Add(1.68572F.ToBaseValue().ToXElement());
            equalRoundedElement.Add(1.68432F.ToBaseValue().ToXElement());
            // act
            var result = equalRounded.Execute(equalRoundedElement, context);
 
            //assert
            Assert.False(result);
        }

    }
}
