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
    public class SubstringTests
    {
        [Fact]
        public void IsSubstring()
        {
            // arrange
            var context = TestHelper.GetDefaultResponseProcessingContext(null);
            context.Operators = new Dictionary<string, IResponseProcessingOperator>();
            context.Expressions = new Dictionary<string, IResponseProcessingExpression>();
   
            var substring = new Substring();

            context.Operators.Add(substring.Name, substring);

            var substringElement = XElement.Parse("<substring></substring>");
            substringElement.Add("Me".ToBaseValue().ToXElement());
            substringElement.Add("TestMe!".ToBaseValue().ToXElement());

            // act
            var result = substring.Execute(substringElement, context);

            //assert
            Assert.True(result);
        }

        [Fact]
        public void IsSubstringInCorrect()
        {
            // arrange
            var context = TestHelper.GetDefaultResponseProcessingContext(null);
            context.Operators = new Dictionary<string, IResponseProcessingOperator>();
            context.Expressions = new Dictionary<string, IResponseProcessingExpression>();

            var substring = new Substring();

            context.Operators.Add(substring.Name, substring);

            var substringElement = XElement.Parse("<substring></substring>");
            substringElement.Add("Not".ToBaseValue().ToXElement());
            substringElement.Add("TestMe!".ToBaseValue().ToXElement());
  
            // act
            var result = substring.Execute(substringElement, context);

            //assert
            Assert.False(result);
        }

        [Fact]
        public void IsSubstringIncorrectCasing()
        {
            // arrange
            var context = TestHelper.GetDefaultResponseProcessingContext(null);
            context.Operators = new Dictionary<string, IResponseProcessingOperator>();
            context.Expressions = new Dictionary<string, IResponseProcessingExpression>();

            var substring = new Substring();

            context.Operators.Add(substring.Name, substring);

            var substringElement = XElement.Parse("<substring></substring>");
            substringElement.Add("me".ToBaseValue().ToXElement());
            substringElement.Add("TestMe!".ToBaseValue().ToXElement());

            // act
            var result = substring.Execute(substringElement, context);

            //assert
            Assert.False(result);
        }


        [Fact]
        public void IsSubstringCorrectCasing()
        {
            // arrange
            var context = TestHelper.GetDefaultResponseProcessingContext(null);
            context.Operators = new Dictionary<string, IResponseProcessingOperator>();
            context.Expressions = new Dictionary<string, IResponseProcessingExpression>();

            var substring = new Substring();

            context.Operators.Add(substring.Name, substring);

            var substringElement = XElement.Parse(@"<substring caseSensitive=""false""></substring>");
            substringElement.Add("me".ToBaseValue().ToXElement());
            substringElement.Add("TestMe!".ToBaseValue().ToXElement());
 
            // act
            var result = substring.Execute(substringElement, context);

            //assert
            Assert.True(result);
        }

    }
}
