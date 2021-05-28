using Citolab.QTI.Scoring.Helper;
using Citolab.QTI.Scoring.ResponseProcessing;
using Citolab.QTI.Scoring.ResponseProcessing.Executors;
using Citolab.QTI.Scoring.Tests;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;
using Xunit;

namespace ScoringEngine.Tests.ResponseProcessingTests.Executors
{
    public class NotTests
    {
        [Fact]
        public void IsFalse()
        {
            // arrange
            var context = TestHelper.GetDefaultResponseProcessingContext(null);
            context.Executors = new Dictionary<string, Citolab.QTI.Scoring.ResponseProcessing.Interfaces.IExecuteReponseProcessing>();

            var returnTrue = new ReturnTrue();
            var not = new Not();

            context.Executors.Add(not.Name, not);
            context.Executors.Add(returnTrue.Name, returnTrue);

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
            context.Executors = new Dictionary<string, Citolab.QTI.Scoring.ResponseProcessing.Interfaces.IExecuteReponseProcessing>();

            var returnFalse = new ReturnFalse();
            var not = new Not();

            context.Executors.Add(not.Name, not);
            context.Executors.Add(returnFalse.Name, returnFalse);

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
            contextInfo.Context.Executors = new Dictionary<string, Citolab.QTI.Scoring.ResponseProcessing.Interfaces.IExecuteReponseProcessing>();

            var returnTrue = new ReturnTrue();
            var not = new Not();

            contextInfo.Context.Executors.Add(not.Name, not);
            contextInfo.Context.Executors.Add(returnTrue.Name, returnTrue);

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
