using Citolab.QTI.ScoringEngine.Helpers;
using Citolab.QTI.ScoringEngine.ResponseProcessing;
using Citolab.QTI.ScoringEngine.Expressions.ConditionExpressions;
using Citolab.QTI.ScoringEngine.Tests;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;
using Xunit;
using Citolab.QTI.ScoringEngine.Interfaces;

namespace ScoringEngine.Tests.ExpressionsTests.ConditionExpressions
{
    public class NotTests
    {
        public NotTests()
        {
            ReturnTrue.Init();
            ReturnFalse.Init();
        }

        [Fact]
        public void IsFalse()
        {
            // arrange
            var context = TestHelper.GetDefaultResponseProcessingContext(null);

            var not = new Not();

            var notElement = XElement.Parse("<qti-not></qti-not>");
            notElement.Add(XElement.Parse("<returnTrue/>"));


            // act
            not.Init(notElement, TestHelper.GetExpressionFactory());
            var result = not.Execute(context);

            //assert
            Assert.False(result);
        }

        [Fact]
        public void IsTrue()
        {
            // arrange
            var context = TestHelper.GetDefaultResponseProcessingContext(null);

            var not = new Not();

            var notElement = XElement.Parse("<qti-not></qti-not>");
            notElement.Add(XElement.Parse("<returnFalse/>"));


            // act
            not.Init(notElement, TestHelper.GetExpressionFactory());
            var result = not.Execute(context);

            //assert
            Assert.True(result);
        }
        [Fact]
        public void ErrorTwoChilds ()
        {
            var contextInfo = TestHelper.GetDefaultResponseProcessingContextAndLogger(null);

            // arrange

            var returnTrue = new ReturnTrue();
            var not = new Not();

            var notElement = XElement.Parse("<qti-not></qti-not>");
            notElement.Add(XElement.Parse("<returnTrue/>"));
            notElement.Add(XElement.Parse("<returnTrue/>"));


            // act
            not.Init(notElement, TestHelper.GetExpressionFactory());
            var result = not.Execute(contextInfo.Context);

            //assert
            contextInfo.MockLog.VerifyLog((state, t) => state.ContainsValue("Not element should contain only one child"), LogLevel.Error, 1);
        }
    }
}
