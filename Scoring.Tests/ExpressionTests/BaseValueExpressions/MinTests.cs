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
    public class MinTests
    {
        [Fact]
        public void QtiMinIsZero()
        {
            var assessmentItem = TestHelper.CreateAssessmentItem(new List<OutcomeDeclaration> { });
            var context = TestHelper.GetDefaultResponseProcessingContext(assessmentItem);

            var minElement = XElement.Parse("<qti-min></qti-min>");
            minElement.Add(0.0F.ToBaseValue().ToXElement());
            minElement.Add(0.0F.ToBaseValue().ToXElement());

            var min = new Min();
            min.Init(minElement, TestHelper.GetExpressionFactory());

            // act
            var score = min.Apply(context);

            Assert.Equal(0, int.Parse(score.Value));
        }
        [Fact]
        public void QtiMinIsOne()
        {
            var assessmentItem = TestHelper.CreateAssessmentItem(new List<OutcomeDeclaration> { });
            var context = TestHelper.GetDefaultResponseProcessingContext(assessmentItem);

            var minElement = XElement.Parse("<qti-min></qti-min>");
            minElement.Add(2.0F.ToBaseValue().ToXElement());
            minElement.Add(1.0F.ToBaseValue().ToXElement());
            minElement.Add(4.0F.ToBaseValue().ToXElement());

            var min = new Min();
            min.Init(minElement, TestHelper.GetExpressionFactory());

            // act
            var score = min.Apply(context);

            Assert.Equal(1, int.Parse(score.Value));
        }

        [Fact]
        public void QtiMinIsZero2()
        {
            var assessmentItem = TestHelper.CreateAssessmentItem(new List<OutcomeDeclaration> { });
            var context = TestHelper.GetDefaultResponseProcessingContext(assessmentItem);

            var minElement = XElement.Parse("<qti-min></qti-min>");
            minElement.Add(3.0F.ToBaseValue().ToXElement());
            minElement.Add(0.0F.ToBaseValue().ToXElement());
            minElement.Add(2.0F.ToBaseValue().ToXElement());

            var min = new Min();
            min.Init(minElement, TestHelper.GetExpressionFactory());

            // act
            var score = min.Apply(context);

            Assert.Equal(0, int.Parse(score.Value));
        }


    }
}
