using Citolab.QTI.Scoring.Helper;
using Citolab.QTI.Scoring.Model;
using Citolab.QTI.Scoring.OutcomeProcessing.Calculators;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;
using Xunit;

namespace Citolab.QTI.Scoring.Tests.OutcomeProcessingTests.Calculators
{
    public class SumTests
    {
        [Fact]
        public void SumBasePlusVariableIsZero()
        {
            var outcomeDeclaration = 0.0F.ToOutcomeDeclaration();
            var baseValue = 0.0F.ToBaseValue();

            var assessmentTest = TestHelper.CreateAssessmentTest("test-toets",new List<OutcomeDeclaration> { outcomeDeclaration });
            var context = TestHelper.GetDefaultOutcomeProcessingContext(assessmentTest);

            var sumElement = XElement.Parse("<sum></sum>");
            sumElement.Add(baseValue.ToXElement());
            sumElement.Add(outcomeDeclaration.ToVariableElement());

            var sum = new Sum();
            var score = sum.Calculate(sumElement,  context);

            Assert.Equal(0, score);
        }

        [Fact]
        public void SumBasePlusVariableIsOne()
        {
            var outcomeDeclaration = 1.0F.ToOutcomeDeclaration();
            var baseValue = 0.0F.ToBaseValue();
            var assessmentTest = TestHelper.CreateAssessmentTest("test-toets", new List<OutcomeDeclaration> { outcomeDeclaration });
            var context = TestHelper.GetDefaultOutcomeProcessingContext(assessmentTest);
            var sumElement = XElement.Parse("<sum></sum>");
            sumElement.Add(baseValue.ToXElement());
            sumElement.Add(outcomeDeclaration.ToVariableElement());
            var sum = new Sum();
            var score = sum.Calculate(sumElement, context);

            Assert.Equal(1, score);
        }

        [Fact]
        public void SumBasePlusVariableIsTwo()
        {
            var outcomeDeclaration = 2.0F.ToOutcomeDeclaration();
            var baseValue = 0.0F.ToBaseValue();

            var assessmentTest = TestHelper.CreateAssessmentTest("test-toets", new List<OutcomeDeclaration> { outcomeDeclaration });
            var context = TestHelper.GetDefaultOutcomeProcessingContext(assessmentTest);
            var sumElement = XElement.Parse("<sum></sum>");
            sumElement.Add(baseValue.ToXElement());
            sumElement.Add(outcomeDeclaration.ToVariableElement());
            var sum = new Sum();
            var score = sum.Calculate(sumElement, context);

            Assert.Equal(2, score);
        }

        [Fact]
        public void SumBasePlusVariableIsThree()
        {
            var outcomeDeclaration = 2.0F.ToOutcomeDeclaration();
            var baseValue = 1.0F.ToBaseValue();

            var assessmentTest = TestHelper.CreateAssessmentTest("test-toets", new List<OutcomeDeclaration> { outcomeDeclaration });
            var context = TestHelper.GetDefaultOutcomeProcessingContext(assessmentTest);
            var sumElement = XElement.Parse("<sum></sum>");
            sumElement.Add(baseValue.ToXElement());
            sumElement.Add(outcomeDeclaration.ToVariableElement());
            var sum = new Sum();
            var score = sum.Calculate(sumElement, context);

            Assert.Equal(3, score);
        }

        [Fact]
        public void SumBaseIsThree()
        {
            var baseValue1 = 1.0F.ToBaseValue();
            var baseValue2 = 2.0F.ToBaseValue("SCORE2");

            var assessmentTest = TestHelper.CreateAssessmentTest("test-toets", new List<OutcomeDeclaration> {  });
            var context = TestHelper.GetDefaultOutcomeProcessingContext(assessmentTest);
            var sumElement = XElement.Parse("<sum></sum>");
            sumElement.Add(baseValue1.ToXElement());
            sumElement.Add(baseValue2.ToXElement());
            var sum = new Sum();
            var score = sum.Calculate(sumElement, context);
            Assert.Equal(3, score);
        }

        [Fact]
        public void SumOutcomesIsThree()
        {
            var outcome1 = 1.0F.ToOutcomeDeclaration();
            var outcome2 = 2.0F.ToOutcomeDeclaration("SCORE2");

            var assessmentTest = TestHelper.CreateAssessmentTest("test-toets", new List<OutcomeDeclaration> { outcome1, outcome2 });
            var context = TestHelper.GetDefaultOutcomeProcessingContext(assessmentTest);
            var sumElement = XElement.Parse("<sum></sum>");
            sumElement.Add(outcome1.ToVariableElement());
            sumElement.Add(outcome2.ToVariableElement());
            var sum = new Sum();
            var score = sum.Calculate(sumElement, context);

            Assert.Equal(3, score);
        }
    }
}
