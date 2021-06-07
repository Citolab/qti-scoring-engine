using Citolab.QTI.ScoringEngine.Helpers;
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

namespace ScoringEngine.Tests.ExpressionsTests.ConditionExpressions
{
    public class EqualTest
    {
        [Fact]
        public void IncorrectAnswer()
        {
            // arrange
            var context = TestHelper.GetDefaultResponseProcessingContext(null);

            var base1 = "1".ToBaseValue();
            var base2 = "0".ToBaseValue();

            var equal = new Equal();
            var equalElement = XElement.Parse(@"<qti-equal tolerance-mode=""exact""></qti-equal>");
            equalElement.Add(base1.ToXElement());
            equalElement.Add(base2.ToXElement());
            // act
            var result = equal.Execute(equalElement, context);

            //assert
            Assert.False(result);
        }

        [Fact]
        public void CorrectAnswer()
        {
            // arrange
            var context = TestHelper.GetDefaultResponseProcessingContext(null);

            var base1 = "1.234".ToBaseValue();
            var base2 = "1.234".ToBaseValue();

            var equal = new Equal();
            var equalElement = XElement.Parse("<qti-equal></qti-equal>");
            equalElement.Add(base1.ToXElement());
            equalElement.Add(base2.ToXElement());
            // act
            var result = equal.Execute(equalElement, context);

            //assert
            Assert.True(result);
        }

        [Fact]
        public void IncorrectAnswerDecimalPoint()
        {
            // arrange
            var context = TestHelper.GetDefaultResponseProcessingContext(null);

            var base1 = "1.234".ToBaseValue();
            var base2 = "1,234".ToBaseValue();

            var equal = new Equal();
            var equalElement = XElement.Parse("<qti-equal></qti-equal>");
            equalElement.Add(base1.ToXElement());
            equalElement.Add(base2.ToXElement());
            // act
            var result = equal.Execute(equalElement, context);

            //assert
            Assert.False(result);
        }

        [Fact]
        public void ErrorNoFloat()
        {
            // arrange
            var contextInfo = TestHelper.GetDefaultResponseProcessingContextAndLogger(null);

            var base1 = "nofloat".ToBaseValue();
            var base2 = "1,234".ToBaseValue();

            var equal = new Equal();
            var equalElement = XElement.Parse("<qti-equal></qti-equal>");
            equalElement.Add(base1.ToXElement());
            equalElement.Add(base2.ToXElement());
            // act
            var result = equal.Execute(equalElement, contextInfo.Context);

            //assert
            contextInfo.MockLog.VerifyLog((state, t) => state.ContainsValue(" - 900001: couldn't convert nofloat and/or 1,234 to float."), LogLevel.Error, 1);
        }

        [Fact]
        public void ErrorUnsupportedToleranceMode()
        {
            // arrange
            var contextInfo = TestHelper.GetDefaultResponseProcessingContextAndLogger(null);

            var base1 = "nofloat".ToBaseValue();
            var base2 = "1,234".ToBaseValue();

            var equal = new Equal();
            var equalElement = XElement.Parse(@"<qti-equal tolerance-mode=""relative""></qti-equal>");
            equalElement.Add(base1.ToXElement());
            equalElement.Add(base2.ToXElement());
            // act
            var result = equal.Execute(equalElement, contextInfo.Context);

            //assert
            contextInfo.MockLog.VerifyLog((state, t) => state.ContainsValue(" - 900001: Unsupported tolerance-mode: relative"), LogLevel.Error, 1);
        }

    }
}
