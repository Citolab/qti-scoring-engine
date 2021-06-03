using Citolab.QTI.ScoringEngine.Helper;
using Citolab.QTI.ScoringEngine.ResponseProcessing;
using Citolab.QTI.ScoringEngine.ResponseProcessing.BooleanExpressions;
using Citolab.QTI.ScoringEngine.Tests;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;
using Xunit;

namespace ScoringEngine.Tests.ResponseProcessingTests.BooleanExpressions
{
    public class NotTests
    {
        [Fact]
        public void IsFalse()
        {
            // arrange
            var context = TestHelper.GetDefaultResponseProcessingContext(null);
            context.BooleanExpressions = new Dictionary<string, Citolab.QTI.ScoringEngine.ResponseProcessing.Interfaces.IBooleanExpression>();

            var returnTrue = new ReturnTrue();
            var not = new Not();

            context.BooleanExpressions.Add(not.Name, not);
            context.BooleanExpressions.Add(returnTrue.Name, returnTrue);

            var andElement = XElement.Parse("<qti-not></qti-not>");
            andElement.Add(XElement.Parse("<returnTrue/>"));

            
            // act
            var result = not.Execute(andElement, context);

            //assert
            Assert.False(result);
        }

        [Fact]
        public void IsTrue()
        {
            // arrange
            var context = TestHelper.GetDefaultResponseProcessingContext(null);
            context.BooleanExpressions = new Dictionary<string, Citolab.QTI.ScoringEngine.ResponseProcessing.Interfaces.IBooleanExpression>();

            var returnFalse = new ReturnFalse();
            var not = new Not();

            context.BooleanExpressions.Add(not.Name, not);
            context.BooleanExpressions.Add(returnFalse.Name, returnFalse);

            var andElement = XElement.Parse("<qti-not></qti-not>");
            andElement.Add(XElement.Parse("<returnFalse/>"));


            // act
            var result = not.Execute(andElement, context);

            //assert
            Assert.True(result);
        }
        [Fact]
        public void ErrorTwoChilds ()
        {
            var contextInfo = TestHelper.GetDefaultResponseProcessingContextAndLogger(null);

            // arrange
            contextInfo.Context.BooleanExpressions = new Dictionary<string, Citolab.QTI.ScoringEngine.ResponseProcessing.Interfaces.IBooleanExpression>();

            var returnTrue = new ReturnTrue();
            var not = new Not();

            contextInfo.Context.BooleanExpressions.Add(not.Name, not);
            contextInfo.Context.BooleanExpressions.Add(returnTrue.Name, returnTrue);

            var andElement = XElement.Parse("<qti-not></qti-not>");
            andElement.Add(XElement.Parse("<returnTrue/>"));
            andElement.Add(XElement.Parse("<returnTrue/>"));


            // act
            var result = not.Execute(andElement, contextInfo.Context);

            //assert
            contextInfo.MockLog.VerifyLog((state, t) => state.ContainsValue("Not element should contain only one child"), LogLevel.Error, 1);
        }
    }
}
