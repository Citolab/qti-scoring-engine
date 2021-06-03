using Citolab.QTI.ScoringEngine.Helper;
using Citolab.QTI.ScoringEngine.ResponseProcessing;
using Citolab.QTI.ScoringEngine.ResponseProcessing.BooleanExpressions;
using Citolab.QTI.ScoringEngine.Tests;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;
using Xunit;

namespace ScoringEngine.Tests.ResponseProcessingTests.BooleanExpressions
{
    public class OrTests
    {
        [Fact]
        public void CheckTwoTrueValues()
        {
            // arrange
            var context = TestHelper.GetDefaultResponseProcessingContext(null);
            context.BooleanExpressions = new Dictionary<string, Citolab.QTI.ScoringEngine.ResponseProcessing.Interfaces.IBooleanExpression>();

            var returnTrue = new ReturnTrue();
            var or = new Or();

            context.BooleanExpressions.Add(or.Name, or);
            context.BooleanExpressions.Add(returnTrue.Name, returnTrue);

            var orElement = XElement.Parse("<qti-or></qti-or>");
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
            context.BooleanExpressions = new Dictionary<string, Citolab.QTI.ScoringEngine.ResponseProcessing.Interfaces.IBooleanExpression>();
            var returnTrue = new ReturnTrue();
            var returnFalse = new ReturnFalse();
            var or = new Or();
            context.BooleanExpressions.Add(or.Name, or);
            context.BooleanExpressions.Add(returnTrue.Name, returnTrue);
            context.BooleanExpressions.Add(returnFalse.Name, returnFalse);

            var orElement = XElement.Parse("<qti-or></qti-or>");
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
            context.BooleanExpressions = new Dictionary<string, Citolab.QTI.ScoringEngine.ResponseProcessing.Interfaces.IBooleanExpression>();
            var returnTrue = new ReturnTrue();
            var returnFalse = new ReturnFalse();

            var or = new Or();
            context.BooleanExpressions.Add(or.Name, or);
            context.BooleanExpressions.Add(returnTrue.Name, returnTrue);
            context.BooleanExpressions.Add(returnFalse.Name, returnFalse);

            var orElement = XElement.Parse("<qti-or></qti-or>");
            orElement.Add(XElement.Parse("<returnFalse/>"));
            orElement.Add(XElement.Parse("<returnFalse/>"));
           
            // act
            var result = or.Execute(orElement, context);
            //assert
            Assert.False(result);
        }
    }
}
