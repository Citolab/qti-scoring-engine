using Citolab.QTI.ScoringEngine.Helpers;
using Citolab.QTI.ScoringEngine.ResponseProcessing;
using Citolab.QTI.ScoringEngine.Expressions.ConditionExpressions;
using Citolab.QTI.ScoringEngine.Tests;
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
    public class OrTests
    {
        public OrTests()
        {
            ReturnTrue.Init();
            ReturnFalse.Init();
        }

        [Fact]
        public void CheckTwoTrueValues()
        {
            // arrange
            var context = TestHelper.GetDefaultResponseProcessingContext(null);

            var or = new Or();
            var orElement = XElement.Parse("<qti-or></qti-or>");
            orElement.Add(XElement.Parse("<returnTrue/>"));
            orElement.Add(XElement.Parse("<returnTrue/>"));
            // act
            or.Init(orElement, TestHelper.GetExpressionFactory());
            var result = or.Execute(context);

            //assert
            Assert.True(result);
        }

        [Fact]
        public void CheckOneTrueAndFalseValues()
        {
            // arrange
            var context = TestHelper.GetDefaultResponseProcessingContext(null);
            var returnTrue = new ReturnTrue();
            var returnFalse = new ReturnFalse();
            var or = new Or();

            var orElement = XElement.Parse("<qti-or></qti-or>");
            orElement.Add(XElement.Parse("<returnTrue/>"));
            orElement.Add(XElement.Parse("<returnFalse/>"));
            // act
            or.Init(orElement, TestHelper.GetExpressionFactory());
            var result = or.Execute(context);
            //assert
            Assert.True(result);
        }

        [Fact]
        public void CheckFalseValues()
        {
            // arrange
            var context = TestHelper.GetDefaultResponseProcessingContext(null);
            var returnTrue = new ReturnTrue();
            var returnFalse = new ReturnFalse();

            var or = new Or();

            var orElement = XElement.Parse("<qti-or></qti-or>");
            orElement.Add(XElement.Parse("<returnFalse/>"));
            orElement.Add(XElement.Parse("<returnFalse/>"));

            // act
            or.Init(orElement, TestHelper.GetExpressionFactory());
            var result = or.Execute(context);
            //assert
            Assert.False(result);
        }
    }
}
