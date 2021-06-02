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
    public class LtTests
    {
        [Fact]
        public void OneGreaterThanZero()
        {
            // arrange
            var context = TestHelper.GetDefaultResponseProcessingContext(null);
            context.Operators = new Dictionary<string, IResponseProcessingOperator>();
            context.Expressions = new Dictionary<string, IResponseProcessingExpression>();
   
            var lt = new Lt();

            context.Operators.Add(lt.Name, lt);

            var ltElement = XElement.Parse("<lt></lt>");
            ltElement.Add(1.0F.ToBaseValue().ToXElement());
            ltElement.Add(0.0F.ToBaseValue().ToXElement());
            // act
            var result = lt.Execute(ltElement, context);

            //assert
            Assert.False(result);
        }
        [Fact]
        public void ZeroSmallerThanOne()
        {
            // arrange
            var context = TestHelper.GetDefaultResponseProcessingContext(null);
            context.Operators = new Dictionary<string, IResponseProcessingOperator>();
            context.Expressions = new Dictionary<string, IResponseProcessingExpression>();

            var lt = new Lt();

            context.Operators.Add(lt.Name, lt);

            var ltElement = XElement.Parse("<lt></lt>");
            ltElement.Add(0.0F.ToBaseValue().ToXElement());
            ltElement.Add(1.0F.ToBaseValue().ToXElement());
            // act
            var result = lt.Execute(ltElement, context);

            //assert
            Assert.True (result);
        }
        [Fact]
        public void EqualReturnsFalse()
        {
            // arrange
            var context = TestHelper.GetDefaultResponseProcessingContext(null);
            context.Operators = new Dictionary<string, IResponseProcessingOperator>();
            context.Expressions = new Dictionary<string, IResponseProcessingExpression>();

            var lt = new Lt();

            context.Operators.Add(lt.Name, lt);

            var ltElement = XElement.Parse("<lt></lt>");
            ltElement.Add(1.0F.ToBaseValue().ToXElement());
            ltElement.Add(1.0F.ToBaseValue().ToXElement());
            // act
            var result = lt.Execute(ltElement, context);

            //assert
            Assert.False(result);
        }
    }
}
