using Citolab.QTI.ScoringEngine.Helpers;
using Citolab.QTI.ScoringEngine.Model;
using Citolab.QTI.ScoringEngine.OutcomeProcessing.BaseValueExpression;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;
using Xunit;

namespace Citolab.QTI.ScoringEngine.Tests.ExpressionTests.BaseValueExpressions
{
    public class MaxTests
    {
        [Fact]
        public void QtiMaxIsZero()
        {
            var assessmentItem = TestHelper.CreateAssessmentItem(new List<OutcomeDeclaration> { });
            var context = TestHelper.GetDefaultResponseProcessingContext(assessmentItem);

            var maxElement = XElement.Parse("<qti-max></qti-max>");
            maxElement.Add(0.0F.ToBaseValue().ToXElement());
            maxElement.Add(0.0F.ToBaseValue().ToXElement());

            var max = new Max();
            max.Init(maxElement, TestHelper.GetExpressionFactory());

            // act
            var score = max.Apply(context);

            Assert.Equal(0, int.Parse(score.Value));
        }
        [Fact]
        public void QtiMaxIsOne()
        {
            var assessmentItem = TestHelper.CreateAssessmentItem(new List<OutcomeDeclaration> { });
            var context = TestHelper.GetDefaultResponseProcessingContext(assessmentItem);

            var maxElement = XElement.Parse("<qti-max></qti-max>");
            maxElement.Add(0.0F.ToBaseValue().ToXElement());
            maxElement.Add(1.0F.ToBaseValue().ToXElement());
            maxElement.Add(0.0F.ToBaseValue().ToXElement());

            var max = new Max();
            max.Init(maxElement, TestHelper.GetExpressionFactory());

            // act
            var score = max.Apply(context);

            Assert.Equal(1, int.Parse(score.Value));
        }

        [Fact]
        public void QtiMaxIsThree()
        {
            var assessmentItem = TestHelper.CreateAssessmentItem(new List<OutcomeDeclaration> { });
            var context = TestHelper.GetDefaultResponseProcessingContext(assessmentItem);

            var maxElement = XElement.Parse("<qti-max></qti-max>");
            maxElement.Add(3.0F.ToBaseValue().ToXElement());
            maxElement.Add(1.0F.ToBaseValue().ToXElement());
            maxElement.Add(2.0F.ToBaseValue().ToXElement());

            var max = new Max();
            max.Init(maxElement, TestHelper.GetExpressionFactory());

            // act
            var score = max.Apply(context);

            Assert.Equal(3, int.Parse(score.Value));
        }


    }
}
