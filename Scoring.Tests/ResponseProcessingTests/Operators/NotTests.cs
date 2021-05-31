using Citolab.QTI.ScoringEngine.Helper;
using Citolab.QTI.ScoringEngine.ResponseProcessing;
using Citolab.QTI.ScoringEngine.ResponseProcessing.Operators;
using Citolab.QTI.ScoringEngine.Tests;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;
using Xunit;

namespace ScoringEngine.Tests.ResponseProcessingTests.Operators
{
    public class NotTests
    {
        [Fact]
        public void IsFalse()
        {
            // arrange
            var context = TestHelper.GetDefaultResponseProcessingContext(null);
            context.Operators = new Dictionary<string, Citolab.QTI.ScoringEngine.ResponseProcessing.Interfaces.IResponseProcessingOperator>();

            var returnTrue = new ReturnTrue();
            var not = new Not();

            context.Operators.Add(not.Name, not);
            context.Operators.Add(returnTrue.Name, returnTrue);

            var andElement = XElement.Parse("<not></not>");
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
            context.Operators = new Dictionary<string, Citolab.QTI.ScoringEngine.ResponseProcessing.Interfaces.IResponseProcessingOperator>();

            var returnFalse = new ReturnFalse();
            var not = new Not();

            context.Operators.Add(not.Name, not);
            context.Operators.Add(returnFalse.Name, returnFalse);

            var andElement = XElement.Parse("<not></not>");
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
            contextInfo.Context.Operators = new Dictionary<string, Citolab.QTI.ScoringEngine.ResponseProcessing.Interfaces.IResponseProcessingOperator>();

            var returnTrue = new ReturnTrue();
            var not = new Not();

            contextInfo.Context.Operators.Add(not.Name, not);
            contextInfo.Context.Operators.Add(returnTrue.Name, returnTrue);

            var andElement = XElement.Parse("<not></not>");
            andElement.Add(XElement.Parse("<returnTrue/>"));
            andElement.Add(XElement.Parse("<returnTrue/>"));


            // act
            var result = not.Execute(andElement, contextInfo.Context);

            //assert
            contextInfo.MockLog.VerifyLog((state, t) => state.ContainsValue("Not element should contain only one child"), LogLevel.Error, 1);
        }
    }
}
