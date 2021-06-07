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
    public class SubstringTests
    {
        [Fact]
        public void IsSubstring()
        {
            // arrange
            var context = TestHelper.GetDefaultResponseProcessingContext(null);
            context.ConditionExpressions = new Dictionary<string, IConditionExpression>();

            var substring = new Substring();

            context.ConditionExpressions.Add(substring.Name, substring);

            var substringElement = XElement.Parse("<qti-substring></qti-substring>");
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
            context.ConditionExpressions = new Dictionary<string, IConditionExpression>();

            var substring = new Substring();

            context.ConditionExpressions.Add(substring.Name, substring);

            var substringElement = XElement.Parse("<qti-substring></qti-substring>");
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
            context.ConditionExpressions = new Dictionary<string, IConditionExpression>();

            var substring = new Substring();

            context.ConditionExpressions.Add(substring.Name, substring);

            var substringElement = XElement.Parse("<qti-substring></qti-substring>");
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
            context.ConditionExpressions = new Dictionary<string, IConditionExpression>();

            var substring = new Substring();

            context.ConditionExpressions.Add(substring.Name, substring);

            var substringElement = XElement.Parse(@"<qti-substring case-sensitive=""false""></qti-substring>");
            substringElement.Add("me".ToBaseValue().ToXElement());
            substringElement.Add("TestMe!".ToBaseValue().ToXElement());
 
            // act
            var result = substring.Execute(substringElement, context);

            //assert
            Assert.True(result);
        }

    }
}
