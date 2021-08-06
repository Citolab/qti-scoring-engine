using Citolab.QTI.ScoringEngine.Helpers;
using Citolab.QTI.ScoringEngine.Model;
using Citolab.QTI.ScoringEngine.OutcomeProcessing.BaseValueExpression;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;
using Xunit;

namespace Citolab.QTI.ScoringEngine.Tests.OutcomeProcessingTests.Expressions
{
    public class RoundTests
    {
        [Fact]
        public void Round6_5()
        {
            var context = TestHelper.GetDefaultResponseProcessingContext(null);
            var baseValue = 6.5F.ToBaseValue().ToXElement();
            var round = new Round();

            var roundElement = XElement.Parse("<qti-round></qti-round>");
            roundElement.Add(baseValue);

            round.Init(roundElement, TestHelper.GetExpressionFactory());
            var score = round.Apply(context);

            var culture = CultureInfo.InvariantCulture;
            Assert.Equal(7, float.Parse(score.Value, culture));
        }

        [Fact]
        public void Round6_8()
        {
            var context = TestHelper.GetDefaultResponseProcessingContext(null);
            var baseValue = 6.8F.ToBaseValue().ToXElement();
            var round = new Round();
            var roundElement = XElement.Parse("<qti-round></qti-round>");
            roundElement.Add(baseValue);
            round.Init(roundElement, TestHelper.GetExpressionFactory());
            var score = round.Apply(context);

            var culture = CultureInfo.InvariantCulture;
            Assert.Equal(7, float.Parse(score.Value, culture));
        }

        [Fact]
        public void Round6_49()
        {
            var context = TestHelper.GetDefaultResponseProcessingContext(null);
            var baseValue = 6.49F.ToBaseValue().ToXElement();
            var round = new Round();
            var roundElement = XElement.Parse("<qti-round></qti-round>");
            roundElement.Add(baseValue);
            round.Init(roundElement, TestHelper.GetExpressionFactory());
            var score = round.Apply(context);

            var culture = CultureInfo.InvariantCulture;
            Assert.Equal(6, float.Parse(score.Value, culture));
        }
    }
}
