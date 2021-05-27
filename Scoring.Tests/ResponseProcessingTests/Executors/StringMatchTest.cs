using Citolab.QTI.Scoring.Helper;
using Citolab.QTI.Scoring.ResponseProcessing.Executors;
using Citolab.QTI.Scoring.Tests;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;
using Xunit;

namespace ScoringEngine.Tests.ResponseProcessingTests.Executors
{
    public class StringMatchTest
    {
        [Fact]
        public void IncorrectAnswer()
        {
            // arrange
            var context = TestHelper.GetDefaultResponseProcessingContext(null);
            //context.Executors = new Dictionary<string, Citolab.QTI.Scoring.ResponseProcessing.Interfaces.IExecuteReponseProcessing>();

            var base1 = "myanswer".ToBaseValue();
            var base2 = "correctAnswer".ToBaseValue();

            //context.Executors.Add(or.Name, or);
            //context.Executors.Add(returnTrue.Name, returnTrue);
            var stringMatch = new StringMatch();
            var stringMatchElement = XElement.Parse("<stringMatch></stringMatch>");
            stringMatchElement.Add(base1.ToXElement());
            stringMatchElement.Add(base2.ToXElement());
            // act
            var result = stringMatch.Execute(stringMatchElement, context);

            //assert
            Assert.False(result);
        }

        [Fact]
        public void CorrectAnswer()
        {
            // arrange
            var context = TestHelper.GetDefaultResponseProcessingContext(null);
            //context.Executors = new Dictionary<string, Citolab.QTI.Scoring.ResponseProcessing.Interfaces.IExecuteReponseProcessing>();

            var base1 = "correctAnswer".ToBaseValue();
            var base2 = "correctAnswer".ToBaseValue();

            //context.Executors.Add(or.Name, or);
            //context.Executors.Add(returnTrue.Name, returnTrue);
            var stringMatch = new StringMatch();
            var stringMatchElement = XElement.Parse("<stringMatch></stringMatch>");
            stringMatchElement.Add(base1.ToXElement());
            stringMatchElement.Add(base2.ToXElement());
            // act
            var result = stringMatch.Execute(stringMatchElement, context);

            //assert
            Assert.True(result);
        }

        [Fact]
        public void IncorrectAnswerCasing()
        {
            // arrange
            var context = TestHelper.GetDefaultResponseProcessingContext(null);
            //context.Executors = new Dictionary<string, Citolab.QTI.Scoring.ResponseProcessing.Interfaces.IExecuteReponseProcessing>();

            var base1 = "correctAnswer".ToBaseValue();
            var base2 = "CorrectAnswer".ToBaseValue();

            //context.Executors.Add(or.Name, or);
            //context.Executors.Add(returnTrue.Name, returnTrue);
            var stringMatch = new StringMatch();
            var stringMatchElement = XElement.Parse("<stringMatch></stringMatch>");
            stringMatchElement.Add(base1.ToXElement());
            stringMatchElement.Add(base2.ToXElement());
            // act
            var result = stringMatch.Execute(stringMatchElement, context);

            //assert
            Assert.False(result);
        }

        [Fact]
        public void IncorrectAnswerCasing2()
        {
            // arrange
            var context = TestHelper.GetDefaultResponseProcessingContext(null);
            //context.Executors = new Dictionary<string, Citolab.QTI.Scoring.ResponseProcessing.Interfaces.IExecuteReponseProcessing>();

            var base1 = "correctAnswer".ToBaseValue();
            var base2 = "CorrectAnswer".ToBaseValue();

            //context.Executors.Add(or.Name, or);
            //context.Executors.Add(returnTrue.Name, returnTrue);
            var stringMatch = new StringMatch();
            var stringMatchElement = XElement.Parse(@"<stringMatch caseSensitive=""true""></stringMatch>");
            stringMatchElement.Add(base1.ToXElement());
            stringMatchElement.Add(base2.ToXElement());
            // act
            var result = stringMatch.Execute(stringMatchElement, context);

            //assert
            Assert.False(result);
        }

        [Fact]
        public void CorrectAnswerCasing()
        {
            // arrange
            var context = TestHelper.GetDefaultResponseProcessingContext(null);
            //context.Executors = new Dictionary<string, Citolab.QTI.Scoring.ResponseProcessing.Interfaces.IExecuteReponseProcessing>();

            var base1 = "correctAnswer".ToBaseValue();
            var base2 = "CorrectAnswer".ToBaseValue();

            //context.Executors.Add(or.Name, or);
            //context.Executors.Add(returnTrue.Name, returnTrue);
            var stringMatch = new StringMatch();
            var stringMatchElement = XElement.Parse(@"<stringMatch caseSensitive=""false""></stringMatch>");
            stringMatchElement.Add(base1.ToXElement());
            stringMatchElement.Add(base2.ToXElement());
            // act
            var result = stringMatch.Execute(stringMatchElement, context);

            //assert
            Assert.True(result);
        }

        [Fact]
        public void IncorrectAnswerFloat()
        {
            // arrange
            var context = TestHelper.GetDefaultResponseProcessingContext(null);
            //context.Executors = new Dictionary<string, Citolab.QTI.Scoring.ResponseProcessing.Interfaces.IExecuteReponseProcessing>();

            var base1 = 1.0F.ToBaseValue();
            var base2 = "1.0".ToBaseValue();

            //context.Executors.Add(or.Name, or);
            //context.Executors.Add(returnTrue.Name, returnTrue);
            var stringMatch = new StringMatch();
            var stringMatchElement = XElement.Parse(@"<stringMatch caseSensitive=""false""></stringMatch>");
            stringMatchElement.Add(base1.ToXElement());
            stringMatchElement.Add(base2.ToXElement());
            // act
            var result = stringMatch.Execute(stringMatchElement, context);

            //assert
            Assert.False(result);
        }

        [Fact]
        public void CorrectAnswerFloat()
        {
            // arrange
            var context = TestHelper.GetDefaultResponseProcessingContext(null);
            //context.Executors = new Dictionary<string, Citolab.QTI.Scoring.ResponseProcessing.Interfaces.IExecuteReponseProcessing>();

            var base1 = 1.0F.ToBaseValue();
            var base2 = "1".ToBaseValue();

            //context.Executors.Add(or.Name, or);
            //context.Executors.Add(returnTrue.Name, returnTrue);
            var stringMatch = new StringMatch();
            var stringMatchElement = XElement.Parse(@"<stringMatch caseSensitive=""false""></stringMatch>");
            stringMatchElement.Add(base1.ToXElement());
            stringMatchElement.Add(base2.ToXElement());
            // act
            var result = stringMatch.Execute(stringMatchElement, context);

            //assert
            Assert.True(result);
        }
    }
}
