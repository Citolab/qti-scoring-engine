using Citolab.QTI.Scoring.Helper;
using Citolab.QTI.Scoring.ResponseProcessing;
using Citolab.QTI.Scoring.ResponseProcessing.Operators;
using Citolab.QTI.Scoring.Tests;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;
using Xunit;

namespace ScoringEngine.Tests.ResponseProcessingTests.Operators
{
    public class OrTests
    {
        [Fact]
        public void CheckTwoTrueValues()
        {
            // arrange
            var context = TestHelper.GetDefaultResponseProcessingContext(null);
            context.Operators = new Dictionary<string, Citolab.QTI.Scoring.ResponseProcessing.Interfaces.IResponseProcessingOperator>();

            var returnTrue = new ReturnTrue();
            var or = new Or();

            context.Operators.Add(or.Name, or);
            context.Operators.Add(returnTrue.Name, returnTrue);

            var orElement = XElement.Parse("<or></or>");
            orElement.Add(XElement.Parse("<returnTrue/>"));
            orElement.Add(XElement.Parse("<returnTrue/>"));
            // act
            var result = or.Execute(orElement, context);

            //assert
            Assert.True(result);
        }

        [Fact]
        public void CheckOneTrueAndFalseValues()
        {
            // arrange
            var context = TestHelper.GetDefaultResponseProcessingContext(null);
            context.Operators = new Dictionary<string, Citolab.QTI.Scoring.ResponseProcessing.Interfaces.IResponseProcessingOperator>();
            var returnTrue = new ReturnTrue();
            var returnFalse = new ReturnFalse();
            var or = new Or();
            context.Operators.Add(or.Name, or);
            context.Operators.Add(returnTrue.Name, returnTrue);
            context.Operators.Add(returnFalse.Name, returnFalse);

            var orElement = XElement.Parse("<or></or>");
            orElement.Add(XElement.Parse("<returnTrue/>"));
            orElement.Add(XElement.Parse("<returnFalse/>"));
            // act
            var result = or.Execute(orElement, context);
            //assert
            Assert.True(result);
        }

        [Fact]
        public void CheckFalseValues()
        {
            // arrange
            var context = TestHelper.GetDefaultResponseProcessingContext(null);
            context.Operators = new Dictionary<string, Citolab.QTI.Scoring.ResponseProcessing.Interfaces.IResponseProcessingOperator>();
            var returnTrue = new ReturnTrue();
            var returnFalse = new ReturnFalse();

            var or = new Or();
            context.Operators.Add(or.Name, or);
            context.Operators.Add(returnTrue.Name, returnTrue);
            context.Operators.Add(returnFalse.Name, returnFalse);

            var orElement = XElement.Parse("<or></or>");
            orElement.Add(XElement.Parse("<returnFalse/>"));
            orElement.Add(XElement.Parse("<returnFalse/>"));
           
            // act
            var result = or.Execute(orElement, context);
            //assert
            Assert.False(result);
        }
    }
}
