using System;
using Citolab.QTI.ScoringEngine.Helper;
using Citolab.QTI.ScoringEngine.ResponseProcessing.Executors;
using Citolab.QTI.ScoringEngine.ResponseProcessing.Interfaces;
using Citolab.QTI.ScoringEngine.Tests;
using System.Collections.Generic;
using System.Xml.Linq;
using Xunit;

namespace ScoringEngine.Tests.ResponseProcessingTests.Executors
{
    public class GteTests
    {
        [Fact]
        public void OneGreaterThanZero()
        {
            // arrange
            var context = TestHelper.GetDefaultResponseProcessingContext(null);
            context.Executors = new Dictionary<string, IExecuteReponseProcessing>();
            context.Calculators = new Dictionary<string, ICalculateResponseProcessing>();
   
            var gte = new Gte();

            context.Executors.Add(gte.Name, gte);

            var gteElement = XElement.Parse("<gte></gte>");
            gteElement.Add(1.0F.ToBaseValue().ToXElement());
            gteElement.Add(0.0F.ToBaseValue().ToXElement());
            // act
            var result = gte.Execute(gteElement, context);

            //assert
            Assert.True(result);
        }

        public void ZeroSmallerThanOne()
        {
            // arrange
            var context = TestHelper.GetDefaultResponseProcessingContext(null);
            context.Executors = new Dictionary<string, IExecuteReponseProcessing>();
            context.Calculators = new Dictionary<string, ICalculateResponseProcessing>();

            var gte = new Gte();

            context.Executors.Add(gte.Name, gte);

            var gteElement = XElement.Parse("<gte></gte>");
            gteElement.Add(0.0F.ToBaseValue().ToXElement());
            gteElement.Add(1.0F.ToBaseValue().ToXElement());
            // act
            var result = gte.Execute(gteElement, context);

            //assert
            Assert.False (result);
        }

        public void EqualReturnsTrue()
        {
            // arrange
            var context = TestHelper.GetDefaultResponseProcessingContext(null);
            context.Executors = new Dictionary<string, IExecuteReponseProcessing>();
            context.Calculators = new Dictionary<string, ICalculateResponseProcessing>();

            var gte = new Gte();

            context.Executors.Add(gte.Name, gte);

            var gteElement = XElement.Parse("<gte></gte>");
            gteElement.Add(1.0F.ToBaseValue().ToXElement());
            gteElement.Add(1.0F.ToBaseValue().ToXElement());
            // act
            var result = gte.Execute(gteElement, context);

            //assert
            Assert.True(result);
        }
    }
}
