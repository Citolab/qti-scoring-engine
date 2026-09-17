using Citolab.QTI.ScoringEngine.Model;
using System.Collections.Generic;
using System.Xml.Linq;
using Xunit;

namespace Citolab.QTI.ScoringEngine.Tests.ExpressionTests.ConditionExpressions
{
    public class LogicOperatorTests
    {
        private const string True = @"<qti-gte><qti-base-value base-type=""float"">1</qti-base-value><qti-base-value base-type=""float"">1</qti-base-value></qti-gte>";
        private const string False = @"<qti-gte><qti-base-value base-type=""float"">0</qti-base-value><qti-base-value base-type=""float"">1</qti-base-value></qti-gte>";

        private static bool Execute(string qtiXml)
        {
            var context = TestHelper.GetDefaultResponseProcessingContext(
                TestHelper.CreateAssessmentItem(new List<OutcomeDeclaration>()));
            var expression = TestHelper.GetExpressionFactory().GetConditionExpression(XElement.Parse(qtiXml), true);
            return expression.Execute(context);
        }

        [Fact]
        public void AnyN_Is_True_Between_Min_And_Max()
        {
            Assert.True(Execute($@"<qti-any-n min=""2"" max=""3"">{True}{True}{False}</qti-any-n>"));
        }

        [Fact]
        public void AnyN_Is_False_Below_Min()
        {
            Assert.False(Execute($@"<qti-any-n min=""2"" max=""3"">{True}{False}{False}</qti-any-n>"));
        }

        [Fact]
        public void AnyN_Is_False_Above_Max()
        {
            Assert.False(Execute($@"<qti-any-n min=""1"" max=""2"">{True}{True}{True}</qti-any-n>"));
        }

        [Fact]
        public void PatternMatch_Matches_The_Whole_Value()
        {
            Assert.True(Execute(@"<qti-pattern-match pattern=""[0-9]{3}""><qti-base-value base-type=""string"">123</qti-base-value></qti-pattern-match>"));
            Assert.False(Execute(@"<qti-pattern-match pattern=""[0-9]{3}""><qti-base-value base-type=""string"">1234</qti-base-value></qti-pattern-match>"));
            Assert.False(Execute(@"<qti-pattern-match pattern=""[0-9]{3}""><qti-base-value base-type=""string"">12a</qti-base-value></qti-pattern-match>"));
        }

        [Fact]
        public void PatternMatch_With_An_Invalid_Pattern_Is_False()
        {
            Assert.False(Execute(@"<qti-pattern-match pattern=""[0-9""><qti-base-value base-type=""string"">123</qti-base-value></qti-pattern-match>"));
        }

        [Fact]
        public void Inside_Checks_A_Point_Against_A_Shape()
        {
            Assert.True(Execute(@"<qti-inside shape=""circle"" coords=""100,100,15""><qti-base-value base-type=""point"">100 105</qti-base-value></qti-inside>"));
            Assert.False(Execute(@"<qti-inside shape=""circle"" coords=""100,100,15""><qti-base-value base-type=""point"">200 200</qti-base-value></qti-inside>"));
        }

        [Fact]
        public void Inside_Supports_A_Rect()
        {
            Assert.True(Execute(@"<qti-inside shape=""rect"" coords=""0,0,100,100""><qti-base-value base-type=""point"">50 50</qti-base-value></qti-inside>"));
            Assert.False(Execute(@"<qti-inside shape=""rect"" coords=""0,0,100,100""><qti-base-value base-type=""point"">150 50</qti-base-value></qti-inside>"));
        }

        [Fact]
        public void DurationLt_Compares_Two_Durations()
        {
            Assert.True(Execute(@"<qti-duration-lt><qti-base-value base-type=""duration"">10.5</qti-base-value><qti-base-value base-type=""duration"">20</qti-base-value></qti-duration-lt>"));
            Assert.False(Execute(@"<qti-duration-lt><qti-base-value base-type=""duration"">20</qti-base-value><qti-base-value base-type=""duration"">20</qti-base-value></qti-duration-lt>"));
        }

        [Fact]
        public void DurationGte_Compares_Two_Durations()
        {
            Assert.True(Execute(@"<qti-duration-gte><qti-base-value base-type=""duration"">20</qti-base-value><qti-base-value base-type=""duration"">20</qti-base-value></qti-duration-gte>"));
            Assert.False(Execute(@"<qti-duration-gte><qti-base-value base-type=""duration"">10</qti-base-value><qti-base-value base-type=""duration"">20</qti-base-value></qti-duration-gte>"));
        }
    }
}
