using Citolab.QTI.ScoringEngine.Interfaces;
using Citolab.QTI.ScoringEngine.Model;
using System.Collections.Generic;
using System.Xml.Linq;
using Xunit;

namespace Citolab.QTI.ScoringEngine.Tests.ExpressionTests.BaseValueExpressions
{
    public class ContainerOperatorTests
    {
        private static BaseValue Apply(string qtiXml)
        {
            var context = TestHelper.GetDefaultResponseProcessingContext(
                TestHelper.CreateAssessmentItem(new List<OutcomeDeclaration>()));
            var expression = TestHelper.GetExpressionFactory().GetValueExpression(XElement.Parse(qtiXml), true);
            return expression.Apply(context);
        }

        private static bool Execute(string qtiXml)
        {
            var context = TestHelper.GetDefaultResponseProcessingContext(
                TestHelper.CreateAssessmentItem(new List<OutcomeDeclaration>()));
            var expression = TestHelper.GetExpressionFactory().GetConditionExpression(XElement.Parse(qtiXml), true);
            return expression.Execute(context);
        }

        [Fact]
        public void Multiple_Builds_A_Container()
        {
            var result = Apply(@"<qti-multiple>
                    <qti-base-value base-type=""identifier"">ChoiceA</qti-base-value>
                    <qti-base-value base-type=""identifier"">ChoiceB</qti-base-value>
                </qti-multiple>");

            Assert.Equal(Cardinality.Multiple, result.Cardinality);
            Assert.Equal(new List<string> { "ChoiceA", "ChoiceB" }, result.Values);
        }

        [Fact]
        public void Multiple_Flattens_Containers_And_Skips_Nulls()
        {
            var result = Apply(@"<qti-multiple>
                    <qti-multiple>
                        <qti-base-value base-type=""identifier"">ChoiceA</qti-base-value>
                        <qti-base-value base-type=""identifier"">ChoiceB</qti-base-value>
                    </qti-multiple>
                    <qti-null/>
                    <qti-base-value base-type=""identifier"">ChoiceC</qti-base-value>
                </qti-multiple>");

            Assert.Equal(new List<string> { "ChoiceA", "ChoiceB", "ChoiceC" }, result.Values);
        }

        [Fact]
        public void ContainerSize_Counts_The_Values()
        {
            var result = Apply(@"<qti-container-size>
                    <qti-multiple>
                        <qti-base-value base-type=""identifier"">ChoiceA</qti-base-value>
                        <qti-base-value base-type=""identifier"">ChoiceB</qti-base-value>
                    </qti-multiple>
                </qti-container-size>");

            Assert.Equal("2", result.Value);
            Assert.Equal(BaseType.Int, result.BaseType);
        }

        [Fact]
        public void ContainerSize_Of_Null_Is_Zero()
        {
            var result = Apply(@"<qti-container-size><qti-null/></qti-container-size>");

            Assert.Equal("0", result.Value);
        }

        [Fact]
        public void Contains_Finds_A_Value()
        {
            Assert.True(Execute(@"<qti-contains>
                    <qti-multiple>
                        <qti-base-value base-type=""identifier"">ChoiceA</qti-base-value>
                        <qti-base-value base-type=""identifier"">ChoiceB</qti-base-value>
                    </qti-multiple>
                    <qti-multiple><qti-base-value base-type=""identifier"">ChoiceA</qti-base-value></qti-multiple>
                </qti-contains>"));
        }

        [Fact]
        public void Contains_Is_False_When_The_Value_Is_Not_There()
        {
            Assert.False(Execute(@"<qti-contains>
                    <qti-multiple>
                        <qti-base-value base-type=""identifier"">ChoiceA</qti-base-value>
                        <qti-base-value base-type=""identifier"">ChoiceB</qti-base-value>
                    </qti-multiple>
                    <qti-multiple><qti-base-value base-type=""identifier"">ChoiceC</qti-base-value></qti-multiple>
                </qti-contains>"));
        }

        [Fact]
        public void Contains_Needs_Every_Value_Of_The_Second_Container()
        {
            Assert.False(Execute(@"<qti-contains>
                    <qti-multiple><qti-base-value base-type=""identifier"">ChoiceA</qti-base-value></qti-multiple>
                    <qti-multiple>
                        <qti-base-value base-type=""identifier"">ChoiceA</qti-base-value>
                        <qti-base-value base-type=""identifier"">ChoiceB</qti-base-value>
                    </qti-multiple>
                </qti-contains>"));
        }

        [Fact]
        public void Contains_Ordered_Finds_A_Subsequence()
        {
            Assert.True(Execute(@"<qti-contains>
                    <qti-ordered>
                        <qti-base-value base-type=""identifier"">A</qti-base-value>
                        <qti-base-value base-type=""identifier"">B</qti-base-value>
                        <qti-base-value base-type=""identifier"">C</qti-base-value>
                    </qti-ordered>
                    <qti-ordered>
                        <qti-base-value base-type=""identifier"">B</qti-base-value>
                        <qti-base-value base-type=""identifier"">C</qti-base-value>
                    </qti-ordered>
                </qti-contains>"));
        }

        [Fact]
        public void Contains_Ordered_Needs_The_Same_Order()
        {
            Assert.False(Execute(@"<qti-contains>
                    <qti-ordered>
                        <qti-base-value base-type=""identifier"">A</qti-base-value>
                        <qti-base-value base-type=""identifier"">B</qti-base-value>
                        <qti-base-value base-type=""identifier"">C</qti-base-value>
                    </qti-ordered>
                    <qti-ordered>
                        <qti-base-value base-type=""identifier"">C</qti-base-value>
                        <qti-base-value base-type=""identifier"">B</qti-base-value>
                    </qti-ordered>
                </qti-contains>"));
        }

        [Fact]
        public void Delete_Removes_Every_Occurrence()
        {
            var result = Apply(@"<qti-delete>
                    <qti-base-value base-type=""identifier"">DoorA</qti-base-value>
                    <qti-multiple>
                        <qti-base-value base-type=""identifier"">DoorA</qti-base-value>
                        <qti-base-value base-type=""identifier"">DoorB</qti-base-value>
                        <qti-base-value base-type=""identifier"">DoorA</qti-base-value>
                        <qti-base-value base-type=""identifier"">DoorC</qti-base-value>
                    </qti-multiple>
                </qti-delete>");

            Assert.Equal(new List<string> { "DoorB", "DoorC" }, result.Values);
        }

        [Fact]
        public void Random_Returns_A_Value_Of_The_Container()
        {
            var result = Apply(@"<qti-random>
                    <qti-multiple>
                        <qti-base-value base-type=""identifier"">ChoiceA</qti-base-value>
                        <qti-base-value base-type=""identifier"">ChoiceB</qti-base-value>
                    </qti-multiple>
                </qti-random>");

            Assert.Equal(Cardinality.Single, result.Cardinality);
            Assert.Contains(result.Value, new[] { "ChoiceA", "ChoiceB" });
        }

        [Fact]
        public void Repeat_Builds_An_Ordered_Container()
        {
            var result = Apply(@"<qti-repeat number-repeats=""3"">
                    <qti-base-value base-type=""identifier"">A</qti-base-value>
                    <qti-base-value base-type=""identifier"">B</qti-base-value>
                </qti-repeat>");

            Assert.Equal(Cardinality.Ordered, result.Cardinality);
            Assert.Equal(new List<string> { "A", "B", "A", "B", "A", "B" }, result.Values);
        }

        [Fact]
        public void Repeat_Without_Repeats_Is_Null()
        {
            Assert.Null(Apply(@"<qti-repeat number-repeats=""0"">
                    <qti-base-value base-type=""identifier"">A</qti-base-value>
                </qti-repeat>"));
        }
    }
}
